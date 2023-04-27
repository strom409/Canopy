using Canopy.API.Common.Repositories;
using Canopy.API.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Canopy.CMT.ActionPlans.Services.Interfaces;
using Canopy.CMT.ActionPlans.Models;
using Canopy.CMT.ActionPlans.Specifications;

namespace Canopy.CMT.ActionPlans.Services
{

    public class ActionPlanDropDownServices : IActionPlanDropDownServices
    {
        #region Private Fields

        private readonly IRepository<ActionPlanGoalCategory> _actionPlanRepo;
        private readonly IRepository<ActionPlanGoalOwnerRole> _actionPlanOwnerRepo;
        private readonly IRepository<StatusCode> _actionPlanStatusRepo;
        private readonly IRepository<StatusCodeType> _actionPlanTypeRepo;
        private readonly IRedisCacheRepository _cache;

        #endregion

        #region Public Constructor

        public ActionPlanDropDownServices(IRepository<ActionPlanGoalCategory> actionPlanRepo,
            IRepository<ActionPlanGoalOwnerRole> actionPlanOwnerRepo, IRepository<StatusCode> actionPlanStatusRepo
            , IRepository<StatusCodeType> actionPlanTypeRepo,
        IRedisCacheRepository cache)
        {
            _actionPlanRepo = actionPlanRepo;
            _actionPlanOwnerRepo = actionPlanOwnerRepo;
            _actionPlanStatusRepo = actionPlanStatusRepo;
            _actionPlanTypeRepo = actionPlanTypeRepo;
            _cache = cache;

        }

        #endregion

        public async Task<List<DropDownDTO>?> GetActionPlanGoalCategories()
        {
            const string cacheKey = "CMT-Action-Plan-Categories-DropDown";
            var results = await _cache.GetByCacheKey<List<DropDownDTO>>(cacheKey);

            if (results != null)
            {
                return results;
            }
            var spec = new EntityByIdSpec<ActionPlanGoalCategory>();
            var data = await _actionPlanRepo.FindAllAsync(spec);

            if (data == null)
            {
                return new List<DropDownDTO> { };
            }

            results = data.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Description)
               .Select(x => new DropDownDTO(x.Id, x.Description)).ToList();
            _ = Task.Run(() => _cache.SaveByCacheKey(results, cacheKey));
            return results;
        }

        public async Task<List<DropDownDTO>?> GetActionPlanGoalOwnerRoles()
        {
            const string cacheKey = $"CMT-Action-Plan-Owner-Goals-DropDown";
            var results = await _cache.GetByCacheKey<List<DropDownDTO>>(cacheKey);

            if (results != null)
            {
                return results;
            }
            var spec = new EntityByIdSpec<ActionPlanGoalOwnerRole>();
            var data = await _actionPlanOwnerRepo.FindAllAsync(spec);
           
            if (data == null)
            {
                return new List<DropDownDTO> { };
            }


            results = data.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Description)
                .Select(x => new DropDownDTO(x.Id, x.Description)).ToList();
            _ = Task.Run(() => _cache.SaveByCacheKey(results, cacheKey));
            return results;
        }

        public async Task<List<DropDownDTO>?> GetStatusCodes()
        {
            const string cacheKey = $"CMT-Status-Codes-DropDown";
            var results = await _cache.GetByCacheKey<List<DropDownDTO>>(cacheKey);

            if (results != null) return results;
           
           var spec=new EntityByIdSpec<StatusCode>();  
            var data = await _actionPlanStatusRepo.FindAllAsync(spec);
            if (data == null)
            {
                return new List<DropDownDTO> { };
            }


            results = data.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Description)
                .Select(x => new DropDownDTO(x.Id, x.Description)).ToList();
            _ = Task.Run(() => _cache.SaveByCacheKey(results, cacheKey));
            return results;
        }

        public async Task<List<DropDownDTO>?> GetStatusCodeTypes()
        {
            const string cacheKey = $"CMT-Status-CodeType-DropDown";
            var results = await _cache.GetByCacheKey<List<DropDownDTO>>(cacheKey);

            if (results != null)
            {
                return results;
            }
            var spec = new EntityByIdSpec<StatusCodeType>();
            var data = await _actionPlanTypeRepo.FindAllAsync(spec);

            if (data == null)
            {
                return new List<DropDownDTO> { };
            }


            results = data.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Description)
                 .Select(x => new DropDownDTO(x.Id, x.Description)).ToList();
            _ = Task.Run(() => _cache.SaveByCacheKey(results, cacheKey));
            return results;
        }

        public async Task RebuildActionPlanCache()
        {
            _cache.RemoveByCacheKey<List<DropDownDTO>>("CMT-Action-Plan-Categories-DropDown");
            _cache.RemoveByCacheKey<List<DropDownDTO>>("CMT-Action-Plan-Owner-Goals-DropDown");
            _cache.RemoveByCacheKey<List<DropDownDTO>>("CMT-Status-Codes-DropDown");
            _cache.RemoveByCacheKey<List<DropDownDTO>>("CMT-Status-CodeType-DropDown");

        }
    }
}

