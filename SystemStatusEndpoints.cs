using Canopy.API.Common;
using Canopy.API.Common.Models.DTO;
using Canopy.Core.Common.Models.DTO;
using Canopy.Core.Common.Services.Interfaces;

namespace Canopy.Core.Common.Endpoints
{
    public static class SystemStatusEndpoints
    {
        public static void MapSystemStatusEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/CommonService/System/ReleaseInfo").WithTags("Canopy Release Notes Endpoints");

            group.MapGet("/{id:int}", GetReleaseInfo)
               .Produces<SystemReleaseNoteDTO>()
               .Produces(StatusCodes.Status401Unauthorized)
               .WithOpenApi(op => new(op)
               {
                   Summary = "Get Release Note",
                   Description = "Returns Release Note for provided ID"
               });

            group.MapGet("/Latest", GetLatestReleaseNote)
               .Produces<SystemReleaseNoteDTO>()
               .Produces(StatusCodes.Status401Unauthorized)
               .WithOpenApi(op => new(op)
               {
                   Summary = "Get Latest Release Note",
                   Description = "Returns latest Release Note"
               });

            group.MapPost("/", AddReleaseInfo)
               .Produces<SystemReleaseNoteDTO>()
               .Produces(StatusCodes.Status401Unauthorized)
               .WithOpenApi(op => new(op)
               {
                   Summary = "Add Release Note",
                   Description = "Add a new system release note"
               });

            group.MapPut("/{id:int}", UpdateReleaseInfo)
             .Produces(StatusCodes.Status204NoContent)
              .Produces(StatusCodes.Status401Unauthorized)
              .Produces<ServiceErrorResponseDTO>(StatusCodes.Status400BadRequest)
              .WithOpenApi(op => new(op)
              {
                  Summary = "Update Release Note",
                  Description = "Update a release note and resets the cache."
              });

            group.MapDelete("/{id:int}", DeleteReleaseInfo)
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status401Unauthorized)
               .Produces<ServiceErrorResponseDTO>(StatusCodes.Status400BadRequest)
               .WithOpenApi(op => new(op)
               {
                   Summary = "Delete Release Note",
                   Description = "Delete a release note and resets the cache."
               });

            group.MapPut("/Cache/Rebuild", ClearCacheReleaseInfo)
               .Produces(StatusCodes.Status204NoContent)
               .Produces<ServiceErrorResponseDTO>(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status401Unauthorized)
               .WithOpenApi(op => new(op)
               {
                   Summary = "Rebuild ReleaseInfo Cache",
                   Description = "Rebuilds the ReleaseInfo cache. Use this after manually updating any data directly in the database."
               });
        }

        public async static Task<IResult> GetReleaseInfo(int id, ISystemStatusService systemStatusService)
        {

            var result = await systemStatusService.GetReleaseInfo(id);

            if (result is ServiceErrorResponseDTO errorDTO)
            {
                return Results.BadRequest(errorDTO);
            }

            return Results.Ok(result);
        }

        public async static Task<IResult> GetLatestReleaseNote(ISystemStatusService statusService)
        {
            var result = await statusService.GetLatestReleaseInfo();
            return Results.Ok(result);
        }

        public async static Task<IResult> AddReleaseInfo(SystemReleaseNoteDTO systemReleaseNotesDTO, ISystemStatusService systemStatusService)
        {

            var result = await systemStatusService.AddReleaseInfo(systemReleaseNotesDTO);

            if (result is ServiceErrorResponseDTO errorDTO)
            {
                return Results.BadRequest(errorDTO);
            }

            return Results.Ok(result);
        }

        public async static Task<IResult> UpdateReleaseInfo(int id, SystemReleaseNoteDTO noteDTO, ISystemStatusService systemStatusService)
        {
            if (noteDTO == null || id != noteDTO.Id) return Results.BadRequest(new ServiceErrorResponseDTO("Invalid request"));

            var result = await systemStatusService.UpdateReleaseInfo(noteDTO);

            if (result is ServiceErrorResponseDTO errorDTO)
            {
                return Results.BadRequest(errorDTO);
            }

            return Results.NoContent();
        }

        public async static Task<IResult> DeleteReleaseInfo(int id, ISystemStatusService systemStatusService)
        {
            var result = await systemStatusService.DeleteReleaseInfo(id);

            if (result is ServiceErrorResponseDTO errorDTO)
            {
                return Results.BadRequest(errorDTO);
            }

            return Results.NoContent();
        }

        public static IResult ClearCacheReleaseInfo(int id ,ISystemStatusService systemStatusService)
        {
            systemStatusService.ResetCacheReleaseInfo(id);
            return Results.NoContent();
        }
    }
}
