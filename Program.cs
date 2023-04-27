using Canopy.API.Common.AuthPolicies;
using Canopy.API.Common.Repositories;
using Canopy.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Canopy.Core.Common.Models;
using Canopy.Core.Common.Services.Interfaces;
using Canopy.Core.Common.Services;
using Canopy.Core.Common.Repositories;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ServiceBus.Interfaces;
using ServiceBus;
using ServiceBusRabbitMQ;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Http.Json;
using Canopy.Core.Common.Authorization.Middleware;
using Canopy.Core.Common.Endpoints;
using Swashbuckle.AspNetCore.SwaggerUI;

var corsAllowAll = "_corsAllowAll";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CommonDbContext>(
        options => options.UseSqlServer("name=ConnectionStrings:SQLClient"));

builder.Services.AddDbContext<CanopyCoreDbContext>(
        options => options.UseSqlServer("name=ConnectionStrings:SQL"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("Canopy.Core.CommonService", new OpenApiInfo { Title = "Canopy.Core.CommonService", Version = "v1" });
    option.CustomSchemaIds(schema => schema.FullName);
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
builder.Services.AddScoped(typeof(IRepository<>), typeof(CommonServiceDbRepository<>));
builder.Services.AddScoped(typeof(ICanopyCoreRepository<>), typeof(CanopyCoreDbRepository<>));
builder.Services.AddScoped<ICommonDropDownService, CommonDropDownService>();
builder.Services.AddScoped<IDataTableBulkEditService, DataTableBulkEditService>();
builder.Services.AddScoped<ISystemErrorLogService, SystemErrorLogService>();
builder.Services.AddScoped<ISystemStatusService, SystemStatusService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("ConnectionStrings:Redis");
    options.InstanceName = "CanopyRedis";
});
builder.Services.AddSingleton<IRedisCacheRepository, CommonServiceRedisRepository>();

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
    var queueName = "Canopy.Core.CommonService";
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
        c.SwaggerEndpoint("/swagger-docs/Canopy.Core.CommonService/swagger.json", "Canopy.Core.CommonService API Docs");
        c.DisplayRequestDuration();
        c.DocExpansion(DocExpansion.None);
    });
}

app.UseMiddleware<ServiceIdentityVerification>();
app.UseAuthorization();

app.MapCommonDropDownEnpoints();
app.MapDataTableBulkEditEndpoints();
app.MapSystemErrorLogEndpoints();
app.MapSystemStatusEndpoints();

app.Run();
