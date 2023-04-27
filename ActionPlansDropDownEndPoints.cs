using Canopy.API.Common;
using Canopy.CMT.ActionPlans.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canopy.CMT.ActionPlans.Endpoints
{
    public static class ActionPlansDropDownEndPoints
    {
        public static void MapActionPlansDropDownEndPoints(this WebApplication app)
        {
            var grp = app.MapGroup("/ActionPlans/DropDown").WithTags("Action Plans  DropDown Endpoints");

            grp.MapGet("/GoalCategories", GetActionPlanGoalCategories)
                 .Produces<List<DropDownDTO>>()
                .Produces(StatusCodes.Status401Unauthorized)
                .WithOpenApi(op => new(op)
                {
                    Summary = "ActionPlans Goal Categories DropDown",
                    Description = "Returns list of values for a ActionPlans GoalCategories Dropdown."
                });

            grp.MapGet("/GoalOwnerRoles", GetActionPlanGoalOwnerRoles)
                .Produces<List<DropDownDTO>>()
               .Produces(StatusCodes.Status401Unauthorized)
               .WithOpenApi(op => new(op)
               {
                   Summary = "ActionPlans Goal OwnerRole DropDown ",
                   Description = "Returns list of values for ActionPlans Goal OwnerRole Dropdown."
               });

            grp.MapGet("/StatusCodes", GetStatusCodes)
               .Produces<List<DropDownDTO>>()
              .Produces(StatusCodes.Status401Unauthorized)
              .WithOpenApi(op => new(op)
              {
                  Summary = "ActionPlans  StatusCodes DropDown",
                  Description = "Returns list of values for a ActionPlans based dropDown StatusCodes ."
              });

            grp.MapGet("/StatusCodeTypes", GetStatusCodeTypes)
             .Produces<List<DropDownDTO>>()
             .Produces(StatusCodes.Status401Unauthorized)
             .WithOpenApi(op => new(op)
             {
                 Summary = "ActionPlans  StatusCodeTypes DropDown",
                 Description = "Returns list of values for a ActionPlans based dropDown StatusCodeTypes."
             });

            grp.MapPut("/Cache/Rebuild", RebuildActionPlanCache)
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status401Unauthorized)
             .WithOpenApi(op => new(op)
             {
                 Summary = "Rebuild ActionPlan Cache",
                 Description = "Rebuild ActionPlan Cache."
             });
        }

        private static async Task<IResult> GetActionPlanGoalCategories(IActionPlanDropDownServices actionPlanGoalCategoriesServices)
        {
            var result = await actionPlanGoalCategoriesServices.GetActionPlanGoalCategories();
            return Results.Ok(result);
        }

        private static async Task<IResult> GetActionPlanGoalOwnerRoles(IActionPlanDropDownServices actionPlanGoalCategoriesServices)
        {
            var result = await actionPlanGoalCategoriesServices.GetActionPlanGoalOwnerRoles();
            return Results.Ok(result);
        }

        private static async Task<IResult> GetStatusCodes(IActionPlanDropDownServices actionPlanGoalCategoriesServices)
        {
            var result = await actionPlanGoalCategoriesServices.GetStatusCodes();
            return Results.Ok(result);
        }

        private static async Task<IResult> GetStatusCodeTypes(IActionPlanDropDownServices actionPlanGoalCategoriesServices)
        {
            var result = await actionPlanGoalCategoriesServices.GetStatusCodeTypes();
            return Results.Ok(result);
        }
        private static async Task<IResult> RebuildActionPlanCache(IActionPlanDropDownServices actionPlanGoalCategoriesServices)
        {
            await actionPlanGoalCategoriesServices.RebuildActionPlanCache();
            return Results.NoContent();
        }

    }
}
