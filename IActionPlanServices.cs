using Canopy.API.Common;
using Canopy.CMT.ActionPlans.Models.DTO;

namespace Canopy.CMT.ActionPlans.Services.Interfaces
{
    public interface IActionPlanServices
    {
        Task<IServiceResponse?> GetActionPlanGoal(int id);
        Task<IServiceResponse?> AddActionPlanGoal(ActionPlanGoalsDTO actionPlanGoalsDTO);
        Task<IServiceResponse?> UpdateActionPlanGoal(int id, ActionPlanGoalsDTO actionPlanGoalsDTO);
        Task DeleteActionPlanGoal(int id);
        Task ClearActionPlanGoalCache(int id);
    }
}
