using Canopy.API.Common.Models.Interfaces;

namespace Canopy.CMT.ActionPlans.Models
{
    public class ActionPlanGoalComment:IEntity,ICreated,ILastUpdated
    {
        public int Id { get; set; }
        public int? ActionPlanGoalId { get; set; }
        public string? Comment { get; set; }
        public DateTime CommentDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public int CreateUserId { get; set; }
        public int LastUpdateUserId { get; set; }
    }
}
