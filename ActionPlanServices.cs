using Canopy.API.Common;
using Canopy.API.Common.Models.DTO;
using Canopy.API.Common.Repositories;
using Canopy.CMT.ActionPlans.Models;
using Canopy.CMT.ActionPlans.Models.DTO;
using Canopy.CMT.ActionPlans.Services.Interfaces;

namespace Canopy.CMT.ActionPlans.Services
{
    public class ActionPlanServices: IActionPlanServices
    {
        private readonly IRepository<ActionPlanGoal> _actionPlanRepo;
        private readonly IRedisCacheRepository _cache;

        public ActionPlanServices(IRepository<ActionPlanGoal> actionPlanRepo, IRedisCacheRepository cache)
        {
            _actionPlanRepo = actionPlanRepo;
            _cache = cache;
        }
        public async Task<IServiceResponse?> GetActionPlanGoal(int id)
        {
            var cacheKey = $"CMT-Action-Plan-{id}";
            var results = await _cache.GetByCacheKey<ActionPlanGoalsDTO>(cacheKey);

            if (results != null)
            {
                return results;
            }

           // var spec = new EntityByIdSpec<Model.Organization>(id);
            var OrgData = await _actionPlanRepo.ListAllAsync();

            if (OrgData == null)
            {
                return new ServiceErrorResponseDTO("Invalid Plan Id");
            }

           // results = await new ActionPlanGoalsDTO().FromModelAsync(OrgData);
            _ = Task.Run(() => _cache.SaveByCacheKey(results, cacheKey));
            return results;
        }
        public async Task<IServiceResponse?> AddActionPlanGoal(ActionPlanGoalsDTO actionPlanGoalsDTO)
        {
            var newPlanobj = new ActionPlanGoal();
            actionPlanGoalsDTO.CopyToModel(newPlanobj);

            await _actionPlanRepo.AddAsync(newPlanobj);

            await ClearActionPlanGoalCache(actionPlanGoalsDTO.Id);

            actionPlanGoalsDTO.Id = newPlanobj.Id;
            return actionPlanGoalsDTO;
        }
        public async Task<IServiceResponse?> UpdateActionPlanGoal(int id, ActionPlanGoalsDTO actionPlanGoalsDTO)
        {
            var actionPlanObj = await _actionPlanRepo.GetByIdAsync(id);

            if (actionPlanObj == null || id != actionPlanGoalsDTO.Id)
            { 
                return null; 
            }

            actionPlanGoalsDTO.CopyToModel(actionPlanObj);
            await _actionPlanRepo.UpdateAsync(actionPlanObj);
            await ClearActionPlanGoalCache(id);
            return actionPlanGoalsDTO;
        }
        public async Task DeleteActionPlanGoal(int id)
        {
            var PlanObj = await _actionPlanRepo.GetByIdAsync(id);

            if (PlanObj == null)
            {
                return;
            }

            await _actionPlanRepo.DeleteAsync(PlanObj);
        }
        public async Task ClearActionPlanGoalCache(int id)
        {
            _cache.RemoveAllByCacheKeyPattern<ActionPlanGoalsDTO>($"CMT-Action-Plan-{id}");
            await GetActionPlanGoal(id);
        }


    }
}
