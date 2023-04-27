using Canopy.API.Common.Models.Interfaces;

namespace Canopy.CMT.ActionPlans.Models
{
    public class ActionPlanGoal :IEntity,ICreated,ILastUpdated
    { 
        public int Id { get; set; }
        public int? Year { get; set; }
        public int? GoalCategoryId { get; set; }
        public string? GoalText { get; set; }
        public int? Rating { get; set; }
        public int? GoalOwnerRoleId { get; set; }
        public int? StatusCodeId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsActive { get; set; }
        public int CreateUserId { get; set; }
        public int LastUpdateUserId { get; set; }
    }
}
