using Canopy.API.Common.Models.DTO;
using Canopy.API.Common;

namespace Canopy.CMT.ActionPlans.Models.DTO
{
    public class ActionPlanGoalCommentDTO : IServiceResponse, IModelToDTO<ActionPlanGoalComment, ActionPlanGoalCommentDTO>
    {
        public int Id { get; set; }
        public int? ActionPlanGoalId { get; set; }
        public string? Comment { get; set; }
        public DateTime CommentDate { get; set; }

        public void CopyToModel(ActionPlanGoalComment model)
        {
            model.ActionPlanGoalId = ActionPlanGoalId;
            model.Comment = Comment;
            model.CommentDate = CommentDate;

        }

        public async Task<ActionPlanGoalCommentDTO> FromModelAsync(ActionPlanGoalComment model, bool populateSubTypes = true)
        {
            this.ActionPlanGoalId = model.ActionPlanGoalId;
            this.Comment = model.Comment;
            this.CommentDate = model.CommentDate;
            return this;
        }
    }
}
