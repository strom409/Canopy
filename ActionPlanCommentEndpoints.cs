using Canopy.CMT.ActionPlans.Models.DTO;
using Canopy.CMT.ActionPlans.Services.Interfaces;

namespace Canopy.CMT.ActionPlans.Endpoints
{
    public static class ActionPlanCommentEndpoints
    {
        public static void MapActionPlanCommentEndPoints(this WebApplication app)
        {
            var group = app.MapGroup("/ActionPlanComment").WithTags("Action Plans Comment  Endpoints");

            group.MapGet("/{Id:int}", GetActionPlanComment)
                  .Produces<ActionPlanGoalCommentDTO>()
                  .Produces(StatusCodes.Status401Unauthorized)
                  .WithOpenApi(op => new(op)
                  {
                      Summary = "Get ActionPlanComment",
                      Description = "Get  Action Plans Comment"
                  });

            group.MapPost("/", AddActionPlanComment)
               .Produces<ActionPlanGoalCommentDTO>()
               .Produces(StatusCodes.Status401Unauthorized)
               .WithOpenApi(op => new(op)
               {
                   Summary = "AddActionPlanComment",
                   Description = "Add Action Plan Comment."
               });

            group.MapPut("/{id:int}", UpdateActionPlanComment)
               .Produces<ActionPlanGoalCommentDTO>()
               .Produces(StatusCodes.Status401Unauthorized)
               .WithOpenApi(op => new(op)
               {
                   Summary = "Update ActionPlanComment",
                   Description = "Update Action Plan Comment for the specified ActionPlanComment ID"
               });


            group.MapDelete("/{id:int}", DeleteActionPlanComment)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi(op => new(op)
                {
                    Summary = "Delete ActionPlanComment",
                    Description = "Delete the ActionPlanComment for the specified ActionPlanComment ID."
                });

            group.MapPut("/Cache/Clear", ClearActionPlanCommentCache)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .WithOpenApi(op => new(op)
                 {
                     Summary = "Clear ActionPlanComment Cache",
                     Description = "Clear ActionPlanComment  cache."
                 });
        }
        private static async Task<IResult> GetActionPlanComment(int id, IActionPlanCommentServices actionPlanCommentServices)
        {
            var results = await actionPlanCommentServices.GetActionPlanComment(id);
            return Results.Ok(results);
        }

        private static async Task<IResult> AddActionPlanComment(ActionPlanGoalCommentDTO actionPlanCommentDTO, IActionPlanCommentServices actionPlanCommentServices)
        {
            var result = await actionPlanCommentServices.AddActionPlanComment(actionPlanCommentDTO);
            return Results.Ok(result);
        }

        private static async Task<IResult> UpdateActionPlanComment(int id, ActionPlanGoalCommentDTO actionPlanCommentDTO, IActionPlanCommentServices actionPlanCommentServices)
        {
            var result = await actionPlanCommentServices.UpdateActionPlanComment(id, actionPlanCommentDTO);
            return Results.Ok(result);
        }

        private static async Task<IResult> DeleteActionPlanComment(int id, IActionPlanCommentServices actionPlanCommentServices)
        {
            await actionPlanCommentServices.DeleteActionPlanComment(id);
            return Results.NoContent();
        }
        private static async Task<IResult> ClearActionPlanCommentCache(int id, IActionPlanCommentServices actionPlanCommentServices)
        {
            await actionPlanCommentServices.ClearActionPlanCommentCache(id);
            return Results.NoContent();
        }
    }
}
