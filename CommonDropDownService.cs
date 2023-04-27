using Canopy.API.Common;
using Canopy.API.Common.Models.DTO;
using Canopy.API.Common.Models.Interfaces;
using Canopy.API.Common.Repositories;
using Canopy.Core.Common.Models;
using Canopy.Core.Common.Services.Interfaces;

namespace Canopy.Core.Common.Services
{
    public class CommonDropDownService : ICommonDropDownService
    {
        #region Private Fields

        private readonly IRepository<StateList> _statesRepo;
        private readonly ICanopyCoreRepository<TimespanList> _timespanRepo;
        private readonly IRedisCacheRepository _cache;

        #endregion

        #region Public Constructor

        public CommonDropDownService(IRepository<StateList> statesRepo, ICanopyCoreRepository<TimespanList> timespanRepo, IRedisCacheRepository cache)
        {
            _cache = cache;
            _statesRepo = statesRepo;
            _timespanRepo = timespanRepo;
        }

        #endregion

        #region Public Methods

        public List<DropDownDTO> GetYesNo()
        {
            return new List<DropDownDTO>
            {
                { new DropDownDTO(1, "Yes") },
                { new DropDownDTO(2, "No") }
            };
        }

        public async Task<List<IDropDownDTO>?> GetStateList(bool useAbbreviations = false)
        {
            var cacheKey = $"Canopy-Core-Common-StateList-Abbreviations-{useAbbreviations}";
            
            List<IDropDownDTO>? results = null;
            if(useAbbreviations)
            {
                results = (await _cache.GetByCacheKey<List<DropDownStringDTO>>(cacheKey))?.ToList<IDropDownDTO>();
            } else
            {
                results = (await _cache.GetByCacheKey<List<DropDownDTO>>(cacheKey))?.ToList<IDropDownDTO>();
            }
            
            if (results != null) return results;

            var data = await _statesRepo.ListAllAsync();

            if (data != null && data.Any())
            {
                if(!useAbbreviations)
                {
                    var dataList = data.Select(x => new DropDownDTO(x.Id, x.Description)).ToList();
                    _cache.SaveByCacheKey(dataList, cacheKey);
                    results = dataList.ToList<IDropDownDTO>();
                } else
                {
                    var dataListStr = data.Select(x => new DropDownStringDTO(x.Abbreviation, x.Description)).ToList();
                    _cache.SaveByCacheKey(dataListStr, cacheKey);
                    results = dataListStr.ToList<IDropDownDTO>();
                }
                
                return results;
            }

            return null;
        }

        public async Task RebuildStateListCache()
        {
            _cache.RemoveAllByCacheKeyPattern<List<DropDownStringDTO>>("Canopy-Core-Common-StateList-Abbreviations");
            _cache.RemoveAllByCacheKeyPattern<List<DropDownDTO>>("Canopy-Core-Common-StateList-Abbreviations");
            _cache.RemoveAllByCacheKeyPattern<List<IDropDownDTO>>("Canopy-Core-Common-StateList-Abbreviations");
            await GetStateList(true);
            await GetStateList(false);
        }

        public async Task<List<DropDownDTO>?> GetTimespanList()
        {
            var cacheKey = "Canopy-Core-Common-TimespanList";

           var results = await _cache.GetByCacheKey<List<DropDownDTO>>(cacheKey);

            if (results != null) return results;

            var data = await _timespanRepo.ListAllAsync();

            if (data != null && data.Any())
            {
                results = data.Select(x => new DropDownDTO(x.Id, x.Description)).ToList();
                _cache.SaveByCacheKey(results, cacheKey);
                return results;
            }

            return null;
        }

        public async Task RebuildTimespanListCache()
        {
            _cache.RemoveAllByCacheKeyPattern<List<DropDownDTO>>("Canopy-Core-Common-TimespanList");
            await GetTimespanList();
        }

        #endregion
    }
}
