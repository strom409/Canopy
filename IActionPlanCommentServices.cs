using Canopy.API.Common;
using Canopy.CMT.ActionPlans.Models.DTO;

namespace Canopy.CMT.ActionPlans.Services.Interfaces
{
    public interface IActionPlanCommentServices
    {
        Task<IServiceResponse?> GetActionPlanComment(int id);
        Task<IServiceResponse?> AddActionPlanComment(ActionPlanGoalCommentDTO actionPlanCommentDTO);
        Task<IServiceResponse?> UpdateActionPlanComment(int id, ActionPlanGoalCommentDTO actionPlanCommentDTO);
        Task DeleteActionPlanComment(int id);
        Task ClearActionPlanCommentCache(int id);
    }
}
