using Canopy.API.Common;
using Canopy.Core.Common.Services.Interfaces;

namespace Canopy.Core.Common.Endpoints
{
    public static class CommonDropDownEnpoints
    {
        public static void MapCommonDropDownEnpoints(this WebApplication app)
        {
            var grp = app.MapGroup("/CommonService/DropDowns").WithTags("Common Canopy Endpoints");

            grp.MapGet("/YesNo", GetYesNo)
                .Produces<List<DropDownDTO>>()
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi(op => new(op)
                {
                    Summary = "Returns Yes/No DropDown",
                    Description = "Returns Yes/No DropDown"
                });

            grp.MapGet("/StateList", GetStateList)
                .Produces<List<DropDownDTO>>()
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi(op => new(op)
                {
                    Summary = "Returns State List DropDown",
                    Description = "Returns DropDown of all the US states"
                });

            grp.MapPut("/StateList/Cache/Update", RebuildStateListCache)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi(op => new(op)
                {
                    Summary = "Rebuild Cache",
                    Description = "Rebuilds the cache for state list DropDown. Use this endpoint after manually updating the data directly in the database."
                });

            grp.MapGet("/Timespans", GetTimespanList)
                .Produces<List<DropDownDTO>>()
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi(op => new(op)
                {
                    Summary = "Returns Timespan List DropDown",
                    Description = "Returns DropDown of all the timespans"
                });

            grp.MapPut("/Timespans/Cache/Rebuild", RebuildTimespanListCache)
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status401Unauthorized)
               .WithOpenApi(op => new(op)
               {
                   Summary = "Rebuild Cache",
                   Description = "Rebuilds the cache for timespans list DropDown. Use this endpoint after manually updating the data directly in the database."
               });
        }

        public static IResult GetYesNo(ICommonDropDownService dropDownService)
        {
            var results = dropDownService.GetYesNo();
            return Results.Ok(results);
        }

        public async static Task<IResult> GetStateList(ICommonDropDownService dropDownService, bool useAbbreviations = false)
        {
            var results = await dropDownService.GetStateList(useAbbreviations);
            return Results.Ok(results);
        }

        public async static Task<IResult> RebuildStateListCache(ICommonDropDownService dropDownService)
        {
            await dropDownService.RebuildStateListCache();
            return Results.NoContent();
        }

        public async static Task<IResult> GetTimespanList(ICommonDropDownService dropDownService)
        {
            var results = await dropDownService.GetTimespanList();
            return Results.Ok(results);
        }

        public async static Task<IResult> RebuildTimespanListCache(ICommonDropDownService dropDownService)
        {
            await dropDownService.RebuildTimespanListCache();
            return Results.NoContent();
        }
    }
}
