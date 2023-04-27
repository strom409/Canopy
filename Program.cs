using Canopy.API.Common;
using Canopy.API.Common.AuthPolicies;
using Canopy.API.Common.Repositories;
using Canopy.CMT.ActionPlans.Authorization.Middleware;
using Canopy.CMT.ActionPlans.Endpoints;
using Canopy.CMT.ActionPlans.Models;
using Canopy.CMT.ActionPlans.Repositories;
using Canopy.CMT.ActionPlans.Services;
using Canopy.CMT.ActionPlans.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using ServiceBus;
using ServiceBus.Interfaces;
using ServiceBusRabbitMQ;

const string corsAllowAll = "_corsAllowAll";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ActionPlanDbContext>(
    options => options.UseSqlServer("name=ConnectionStrings:SQLClient"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("Canopy.CMT.ActionPlans", new OpenApiInfo { Title = "Canopy.CMT.ActionPlans", Version = "v1" });
    option.CustomSchemaIds(schema => schema.FullName?.Replace("+", "."));
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsAllowAll,
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});

builder.Services.AddSingleton<IJWTUtils, JWTUtils>();
builder.Services.AddTransient<IAuthorizationHandler, SystemAdminPolicyHandler>();
builder.Services.AddTransient<IAuthorizationHandler, AdminPolicyHandler>();
builder.Services.AddTransient<IAuthorizationHandler, CanopyEditorPolicyHandler>();
builder.Services.AddTransient<IAuthorizationHandler, CanopyViewerPolicyHandler>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(ActionPlanDbRepository<>));
builder.Services.AddScoped<IActionPlanDropDownServices, ActionPlanDropDownServices>();
builder.Services.AddScoped<IActionPlanServices, ActionPlanServices>();
builder.Services.AddScoped<IActionPlanCommentServices, ActionPlanCommentServices>();


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("ConnectionStrings:Redis");
    options.InstanceName = "CanopyRedis";
});
builder.Services.AddSingleton<IRedisCacheRepository, ActionPlanRedisRepository>();

builder.Services.AddSingleton<IRabbitMQPersistentConnection>(svcProvider =>
{
    var logger = svcProvider.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

    var factory = new ConnectionFactory()
    {
        HostName = builder.Configuration.GetValue<string>("RabbitMQ:Server"),
        Port = builder.Configuration.GetValue<int>("RabbitMQ:Port"),
        UserName = builder.Configuration.GetValue<string>("RabbitMQ:Username"),
        Password = builder.Configuration.GetValue<string>("RabbitMQ:Password")
    };

    var retryCnt = 5;

    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCnt);
});

builder.Services.AddSingleton<IServiceBus, ServiceBusRabbitMQ.ServiceBusRabbitMQ>(svcProvider =>
{
    var queueName = "Canopy.CMT.ActionPlans";
    var rabbitMQConn = svcProvider.GetRequiredService<IRabbitMQPersistentConnection>();
    var logger = svcProvider.GetRequiredService<ILogger<ServiceBusRabbitMQ.ServiceBusRabbitMQ>>();
    var serviceBusSubMgr = svcProvider.GetRequiredService<IServiceBusSubscriptionsManager>();
    var retryCnt = 5;

    return new ServiceBusRabbitMQ.ServiceBusRabbitMQ(rabbitMQConn, logger, svcProvider, serviceBusSubMgr, queueName, retryCnt);
});
builder.Services.AddSingleton<IServiceBusSubscriptionsManager, InMemoryServiceBusSubscriptionsManager>();

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SystemAdminPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new SystemAdminPolicy());
    });
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new AdminPolicy());
    });
    options.AddPolicy("CanopyEditorPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new CanopyEditorPolicy());
    });
    options.AddPolicy("CanopyViewerPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new CanopyViewerPolicy());
    });
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        //.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser().Build();
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

app.UseCors(corsAllowAll);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger-docs/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger-docs/Canopy.CMT.ActionPlans/swagger.json", "Canopy.CMT.ActionPlans API Docs");
    });
}

app.UseMiddleware<ServiceIdentityVerification>();
app.UseAuthorization();
app.MapActionPlansDropDownEndPoints();
app.MapActionPlansEndPoints();
app.MapActionPlanCommentEndPoints();
app.Run();