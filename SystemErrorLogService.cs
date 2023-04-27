using Canopy.API.Common;
using Canopy.API.Common.Models.DTO;
using Canopy.API.Common.Repositories;
using Canopy.Core.Common.Models;
using Canopy.Core.Common.Models.DTO;
using Canopy.Core.Common.Services.Interfaces;
using Canopy.Core.Common.Specifications;
using System.Text;

namespace Canopy.Core.Common.Services
{
    public class SystemErrorLogService : ISystemErrorLogService
    {
        #region Private Fields

        private readonly ICanopyCoreRepository<SystemErrorLog> _errorLogRepo;
        private readonly IRedisCacheRepository _cache;

        #endregion

        #region Public Constructors

        public SystemErrorLogService(ICanopyCoreRepository<SystemErrorLog> errorLogRepo, 
            IRedisCacheRepository cache )
        {  
           _errorLogRepo = errorLogRepo;
           _cache = cache; 
        }

        #endregion

        #region Public Methods

        public async Task<IServiceResponse?> AddErrorLog(SystemErrorLogDTO systemErrorLogDTO)
        {
            var newRecord = new SystemErrorLog();

            try
            {
                systemErrorLogDTO.CopyToModel(newRecord);
                newRecord.IsActive = true;
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponseDTO(ex.Message);
            }

            await _errorLogRepo.AddAsync(newRecord);
            systemErrorLogDTO.Id = newRecord.Id;

            ResetErrorLogCache();
            
            return systemErrorLogDTO;
        }

        public async Task<DataDTOPagingDTO<SystemErrorLog, SystemErrorLogDTO>?> GetSystemErrorLogs(SystemErrorLogFilterDTO filterDTO)
        {
            //define default filter, if needed (is part of cache key)
            if (filterDTO.SortColumns == null || !filterDTO.SortColumns.Any())
            {
                filterDTO.SortColumns = new List<GenericDataTableSort>
                {
                    new GenericDataTableSort { ColumnId = "CreateDate", SortDirection = "DESC"}
                };
            }

            var cacheKey = GetCacheKey("CommonService-Error-Logs-List", filterDTO);
            var results = await _cache.GetByCacheKey<DataDTOPagingDTO<SystemErrorLog, SystemErrorLogDTO>>(cacheKey);

            if (results != null) return results;

            var spec = new SystemErroLogByUserIdSpec(filterDTO.CreateUserIds)
                         .And(new SystemErroLogByClientIdSpec(filterDTO.ClientIds))
                         .And(new SystemErroLogByAppIdSpec(filterDTO.ApplicationIds))
                         .And(new SystemErroLogByAppPageIdSpec(filterDTO.AppPageIds));
            spec.AddSortColumns(filterDTO.SortColumns);
            spec.EnablePaging(filterDTO.PageSize!.Value, filterDTO.PageNumber!.Value);
            results = await _errorLogRepo.PageAllToDTOAsync<SystemErrorLogDTO>(spec);

            if (results != null)
            {
                _cache.SaveByCacheKey(results, cacheKey);
                return results;
            }

            return null;
        }

        public async Task<IServiceResponse?> DeleteErrorLog(int id)
        {
            var record = await _errorLogRepo.GetByIdAsync(id);

            if (record == null)
            {
                return new ServiceErrorResponseDTO("Invalid error log ID");
            }

            record.IsActive = false;

            await _errorLogRepo.UpdateAsync(record);

            ResetErrorLogCache();

            return null;
        }

        public void ResetErrorLogCache()
        {
             _cache.RemoveAllByCacheKeyPattern<DataDTOPagingDTO<SystemErrorLog, SystemErrorLogDTO>>("CommonService-Error-Logs-List");
        }

        #endregion

        #region Private Methods

        private string GetCacheKey(string prefix, SystemErrorLogFilterDTO filterDTO)
        {
            var createUsers = string.Empty;
            if(filterDTO.CreateUserIds != null && filterDTO.CreateUserIds.Any())
            {
                createUsers = string.Join(',', filterDTO.CreateUserIds);
            }

            var clients = string.Empty;
            if(filterDTO.ClientIds != null && filterDTO.ClientIds.Any())
            {
                clients = string.Join(',', filterDTO.ClientIds);
            }

            var apps = string.Empty;
            if(filterDTO.ApplicationIds != null && filterDTO.ApplicationIds.Any())
            {
                apps = string.Join(',', filterDTO.ApplicationIds);
            }

            var appPages = string.Empty;
            if(filterDTO.AppPageIds != null && filterDTO.AppPageIds.Any())
            {
                appPages = string.Join(',', filterDTO.AppPageIds);
            }

            var sortKeys = new StringBuilder();
            foreach (var colKey in filterDTO.SortColumns)
            {
                sortKeys.Append($"{colKey.ColumnId}:{colKey.SortDirection}");
            }

            var key = $"{prefix}-{createUsers}-{clients}-{apps}-{appPages}" +
                $"-{filterDTO.LayoutID}-{filterDTO.PageNumber}" +
                $"-{filterDTO.PageSize}-{filterDTO.SearchText}-{sortKeys}";

            return key;
        }

        #endregion
    }
}
