using Canopy.API.Common;
using Canopy.API.Common.Models.DTO;
using Canopy.Core.Common.Models.DTO;
using Canopy.Core.Common.Services.Interfaces;

namespace Canopy.Core.Common.Endpoints
{
    public static class DataTableBulkEditEndpoints
    {
        public static void MapDataTableBulkEditEndpoints(this WebApplication app)
        {
            var grp = app.MapGroup("/CommonService/BulkEdit").WithTags("Data Table Batch Edit Endpoints");

            grp.MapGet("/Columns", GetBatchEditDropDown)
                .Produces<List<DropDownDTO>>()
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi(op => new(op)
                {
                    Summary = "List of editable fields",
                    Description = "Returns list of editable fields for the given appPageID and layoutID."
                });

            grp.MapPost("/Update", BatchEditUpdate)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi(op => new(op)
                {
                    Summary = "Apply batch updates",
                    Description = "Applies the update to the underlying database table for the given column."
                });
        }

        public static async Task<IResult> GetBatchEditDropDown(int appPageID, int layoutID, IDataTableBulkEditService editService)
        {
            var results = await editService.GetBatchEditFieldOptions(appPageID, layoutID);
            return Results.Ok(results);
        }

        public static async Task<IResult> BatchEditUpdate(DataTableBatchEditUpdateDTO updateDTO, IDataTableBulkEditService editService)
        {
            var result = await editService.BatchEditUpdate(updateDTO);

            if(result is ServiceErrorResponseDTO errorResponseDTO)
            {
                return Results.BadRequest(errorResponseDTO);
            }

            return Results.NoContent();
        }
    }
}
