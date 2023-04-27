using Canopy.API.Common.Models.DTO;
using Canopy.API.Common;

namespace Canopy.CMT.ActionPlans.Models.DTO
{
    public class ActionPlanGoalsDTO : IServiceResponse, IModelToDTO<ActionPlanGoal, ActionPlanGoalsDTO>
    {
        public int Id { get; set; }
        public int? Year { get; set; }
        public int? GoalCategoryId { get; set; }
        public string? GoalText { get; set; }
        public int? Rating { get; set; }
        public int? GoalOwnerRoleId { get; set; }
        public int? StatusCodeId { get; set; }
        public bool IsActive { get; set; }

        public void CopyToModel(ActionPlanGoal model)
        {
            model.Year = Year;
            model.GoalCategoryId = GoalCategoryId;
            model.GoalText = GoalText;
            model.Rating = Rating;
            model.GoalOwnerRoleId = GoalOwnerRoleId;
            model.StatusCodeId = StatusCodeId;
            model.IsActive = IsActive;
        }

        public async Task<ActionPlanGoalsDTO> FromModelAsync(ActionPlanGoal model, bool populateSubTypes = true)
        {
            this.Year = model.Year;
            this.GoalCategoryId = model.GoalCategoryId;
            this.GoalText = model.GoalText;
            this.Rating = model.Rating;
            this.GoalOwnerRoleId = model.GoalOwnerRoleId;
            this.StatusCodeId = model.StatusCodeId;
            this.IsActive = model.IsActive;
            return this;
        }
    }
}
