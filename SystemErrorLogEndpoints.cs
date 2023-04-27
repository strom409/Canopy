using Canopy.API.Common.Models.DTO;
using Canopy.API.Common;
using Canopy.Core.Common.Services.Interfaces;
using Canopy.Core.Common.Models.DTO;
using Canopy.Core.Common.Services;

namespace Canopy.Core.Common.Endpoints
{
    public static class SystemErrorLogEndpoints
    {
        public static void MapSystemErrorLogEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("CommonService/ErrorLog").WithTags("System Error Log Endpoints");

            group.MapPost("/", AddErrorLog)
                   .Produces<SystemErrorLogDTO>()
                   .Produces(StatusCodes.Status401Unauthorized)
                   .Produces<ServiceErrorResponseDTO>(StatusCodes.Status400BadRequest)
                   .WithOpenApi(op => new(op)
                   {
                        Summary = "Log System Error",
                        Description = "Log new system error"
                   });

            group.MapDelete("/{id:int}", DeleteErrorLog)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces<ServiceErrorResponseDTO>(StatusCodes.Status400BadRequest)
                .WithOpenApi(op => new(op)
                {
                     Summary = "Delete error log",
                     Description = "Deletes an error log and resets cache."
                });

            group.MapPost("/Filter", GetErrorLogs)
               .Produces<List<SystemErrorLogDTO>>()
               .Produces(StatusCodes.Status401Unauthorized)
               .Produces<ServiceErrorResponseDTO>(StatusCodes.Status400BadRequest)
               .WithOpenApi(op => new(op)
               {
                   Summary = "List error logs",
                   Description = "Returns list of error logs based on the provided filter."
               });

            group.MapPut("/Cache/Clear", ClearErrorLogCache)
               .Produces(StatusCodes.Status204NoContent)
               .Produces<ServiceErrorResponseDTO>(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status401Unauthorized)
               .WithOpenApi(op => new(op)
               {
                   Summary = "Clear Cache",
                   Description = "Clears the error log cache. Use this after manually updating any data directly in the database."
               });

        }
        public async static Task<IResult> AddErrorLog(SystemErrorLogDTO systemErrorLogDTO, ISystemErrorLogService commonService)
        {
            var result = await commonService.AddErrorLog(systemErrorLogDTO);

            if (result is ServiceErrorResponseDTO errorDTO)
            {
                return Results.BadRequest(errorDTO);
            }

            return Results.Ok(result);
        }

        public async static Task<IResult> DeleteErrorLog(int id, ISystemErrorLogService commonService)
        {
            var result = await commonService.DeleteErrorLog(id);

            if (result is ServiceErrorResponseDTO errorDTO)
            {
                return Results.BadRequest(errorDTO);
            }

            return Results.NoContent();
        }

        public async static Task<IResult> GetErrorLogs(SystemErrorLogFilterDTO systemErrorLogFilterDTO, ISystemErrorLogService commonService)
        {
            var result= await commonService.GetSystemErrorLogs(systemErrorLogFilterDTO);
           
            if (result == null)
            {
                return Results.BadRequest(new ServiceErrorResponseDTO("Invalid request"));
            }

            return Results.Ok(result);
        }

        public static IResult ClearErrorLogCache(ISystemErrorLogService commonService)
        {
            commonService.ResetErrorLogCache();
            return Results.NoContent();
        }
    }
}
