using Canopy.API.Common;
using Canopy.API.Common.Models.DTO;
using Canopy.API.Common.Repositories;
using Canopy.CMT.ActionPlans.Models;
using Canopy.CMT.ActionPlans.Models.DTO;
using Canopy.CMT.ActionPlans.Services.Interfaces;

namespace Canopy.CMT.ActionPlans.Services
{
    public class ActionPlanCommentServices: IActionPlanCommentServices
    {
        private readonly IRepository<ActionPlanGoalComment> _actionPlancomment;
        private readonly IRedisCacheRepository _cache;
        public ActionPlanCommentServices(IRepository<ActionPlanGoalComment> actionPlancomment, IRedisCacheRepository cache)
        {
            _actionPlancomment = actionPlancomment;
            _cache = cache;
        }

        public async Task<IServiceResponse?> GetActionPlanComment(int id)
        {
            var cacheKey = $"CMT-Action-Plan-Comment-{id}";
            var results = await _cache.GetByCacheKey<ActionPlanGoalCommentDTO>(cacheKey);

            if (results != null)
            {
                return results;
            }

            // var spec = new EntityByIdSpec<Model.Organization>(id);
            var OrgData = await _actionPlancomment.ListAllAsync();

            if (OrgData == null)
            {
                return new ServiceErrorResponseDTO("Invalid Comment Id");
            }

            // results = await new ActionPlanGoalsDTO().FromModelAsync(OrgData);
            _ = Task.Run(() => _cache.SaveByCacheKey(results, cacheKey));
            return results;
        }

        public async Task<IServiceResponse?> AddActionPlanComment(ActionPlanGoalCommentDTO actionPlanCommentDTO)
        {
            var newCommentobj = new ActionPlanGoalComment();
            actionPlanCommentDTO.CopyToModel(newCommentobj);

            await _actionPlancomment.AddAsync(newCommentobj);

            await ClearActionPlanCommentCache(actionPlanCommentDTO.Id);

            actionPlanCommentDTO.Id = newCommentobj.Id;
            return actionPlanCommentDTO;
        }

        public async Task<IServiceResponse?> UpdateActionPlanComment(int id, ActionPlanGoalCommentDTO actionPlanCommentDTO)
        {
            var actionCommentObj = await _actionPlancomment.GetByIdAsync(id);

            if (actionCommentObj == null || id != actionPlanCommentDTO.Id)
            {
                return null;
            }

            actionPlanCommentDTO.CopyToModel(actionCommentObj);
            await _actionPlancomment.UpdateAsync(actionCommentObj);
            await ClearActionPlanCommentCache(id);
            return actionPlanCommentDTO;
        }


        public async Task DeleteActionPlanComment(int id)
        {
            var commentObj = await _actionPlancomment.GetByIdAsync(id);

            if (commentObj == null)
            {
                return;
            }

            await _actionPlancomment.DeleteAsync(commentObj);
        }

       
        public async Task ClearActionPlanCommentCache(int id)
        {
            _cache.RemoveAllByCacheKeyPattern<ActionPlanGoalCommentDTO>($"CMT-Action-Plan-Comment-{id}");
            await GetActionPlanComment(id);
        }
    }
   
}
