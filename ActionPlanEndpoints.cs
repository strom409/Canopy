using Canopy.API.Common;
using Canopy.CMT.ActionPlans.Models.DTO;
using Canopy.CMT.ActionPlans.Services.Interfaces;

namespace Canopy.CMT.ActionPlans.Endpoints
{
    public static class ActionPlanEndpoints
    {
        public static void MapActionPlansEndPoints(this WebApplication app)
        {
            var group = app.MapGroup("/ActionPlans").WithTags("Action Plans  Endpoints");

            group.MapGet("/{Id:int}", GetActionPlanGoal)
                  .Produces<List<ActionPlanGoalsDTO>>()
                  .Produces(StatusCodes.Status401Unauthorized)
                  .WithOpenApi(op => new(op)
                  {
                      Summary = "Get ActionPlanGoal",
                      Description = "Get  Action Plan Goal"
                  });

            group.MapPost("/", AddActionPlanGoal)
               .Produces<ActionPlanGoalsDTO>()
               .Produces(StatusCodes.Status401Unauthorized)
               .WithOpenApi(op => new(op)
               {
                   Summary = "AddActionPlanGoal",
                   Description = "Add Action Plan Goal."
               });

            group.MapPut("/{id:int}", UpdateActionPlanGoal)
               .Produces<ActionPlanGoalsDTO>()
               .Produces(StatusCodes.Status401Unauthorized)
               .WithOpenApi(op => new(op)
               {
                   Summary = "Update ActionPlanGoal",
                   Description = "Update Action Plan Goal for the specified ActionPlanGoal ID"
               });


            group.MapDelete("/{id:int}", DeleteActionPlanGoal)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi(op => new(op)
                {
                    Summary = "Delete ActionPlanGoal",
                    Description = "Delete the ActionPlanGoal for the specified ActionPlanGoal ID."
                });

            group.MapPut("/Cache/Clear", ClearActionPlanGoalCache)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .WithOpenApi(op => new(op)
                 {
                     Summary = "Clear ActionPlanGoal Cache",
                     Description = "Clear ActionPlanGoal  cache."
                 });
        }
        private static async Task<IResult> GetActionPlanGoal(int id, IActionPlanServices actionPlanServices)
        {
            var results = await actionPlanServices.GetActionPlanGoal(id);
            return Results.Ok(results);
        }

        private static async Task<IResult> AddActionPlanGoal(ActionPlanGoalsDTO actionPlanGoalsDTO, IActionPlanServices regionsService)
        {
            var result = await regionsService.AddActionPlanGoal(actionPlanGoalsDTO);
            return Results.Ok(result);
        }

        private static async Task<IResult> UpdateActionPlanGoal(int id, ActionPlanGoalsDTO actionPlanGoalsDTO, IActionPlanServices regionsService)
        {
            var result = await regionsService.UpdateActionPlanGoal(id, actionPlanGoalsDTO);
            return Results.Ok(result);
        }

        private static async Task<IResult> DeleteActionPlanGoal(int id, IActionPlanServices regionsService)
        {
            await regionsService.DeleteActionPlanGoal(id);
            return Results.NoContent();
        }
        private static async Task<IResult> ClearActionPlanGoalCache(int id, IActionPlanServices regionsService)
        {
            await regionsService.ClearActionPlanGoalCache(id);
            return Results.NoContent();
        }


    }
}
