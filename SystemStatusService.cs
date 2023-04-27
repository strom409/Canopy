using Canopy.API.Common;
using Canopy.API.Common.Models.DTO;
using Canopy.API.Common.Repositories;
using Canopy.Core.Common.Models;
using Canopy.Core.Common.Models.DTO;
using Canopy.Core.Common.Services.Interfaces;

namespace Canopy.Core.Common.Services
{
    public class SystemStatusService : ISystemStatusService
    {
        #region Private Fields

        private readonly ICanopyCoreRepository<SystemReleaseNote> _notesRepo;
        private readonly IRedisCacheRepository _cache;

        #endregion

        #region Public Constructor

        public SystemStatusService(ICanopyCoreRepository<SystemReleaseNote> notesRepo,
             IRedisCacheRepository cache)
        {
            _notesRepo = notesRepo;
            _cache = cache;
        }

        #endregion

        #region Public Methods

        public async Task<IServiceResponse> AddReleaseInfo(SystemReleaseNoteDTO systemReleaseNotesDTO)
        {
            var newRecord = new SystemReleaseNote();

            try
            {
                systemReleaseNotesDTO.CopyToModel(newRecord);    
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponseDTO(ex.Message);
            }

            await _notesRepo.AddAsync(newRecord);
            systemReleaseNotesDTO.Id = newRecord.Id;

            await ResetCacheReleaseInfo(newRecord.Id);
            
            return systemReleaseNotesDTO;
        }
       
        public async Task<IServiceResponse?> GetReleaseInfo(int id)
        {
            var cacheKey = $"CommonService-System-Release-Info-{id}";
            var results = await _cache.GetByCacheKey<SystemReleaseNoteDTO>(cacheKey);

            if (results != null) return results;

            var data = await _notesRepo.GetByIdAsync(id);

            if (data != null)
            {
                results = await new SystemReleaseNoteDTO().FromModelAsync(data);
                _= Task.Run(() => _cache.SaveByCacheKey(results, cacheKey));
                return results;
            }

            return new ServiceErrorResponseDTO("Invalid System Release Note Id");
        }

        public async Task<SystemReleaseNoteDTO?> GetLatestReleaseInfo()
        {
            var cacheKey = "CommonService-System-Release-Info-Latest";
            var result = await _cache.GetByCacheKey<SystemReleaseNoteDTO>(cacheKey);

            if(result != null) return result;

            var data = await _notesRepo.ListAllAsync();

            if(data != null)
            {
                result = await new SystemReleaseNoteDTO().FromModelAsync(data.OrderByDescending(x => x.InstallDate).FirstOrDefault()!);
                _ = Task.Run(() => _cache.SaveByCacheKey(result, cacheKey));
                return result;
            }

            return null;
        }

        public async Task<IServiceResponse?> UpdateReleaseInfo(SystemReleaseNoteDTO noteDTO)
        {
            var record = await _notesRepo.GetByIdAsync(noteDTO.Id);

            if (record == null)
            {
                return new ServiceErrorResponseDTO("Invalid release note ID");
            }

            try
            {
                noteDTO.CopyToModel(record);
            } catch (Exception ex)
            {
                return new ServiceErrorResponseDTO(ex.Message);
            }

            await _notesRepo.UpdateAsync(record);
            await  ResetCacheReleaseInfo(noteDTO.Id);

            return null;
        }

        public async Task<IServiceResponse?> DeleteReleaseInfo(int id)
        {
            var record = await _notesRepo.GetByIdAsync(id);

            if (record == null)
            {
                return new ServiceErrorResponseDTO("Invalid release note ID");
            }

            await _notesRepo.DeleteAsync(record);
            await ResetCacheReleaseInfo(id);

            return null;    
        }

        public async Task ResetCacheReleaseInfo(int id)
        {
            _cache.RemoveByCacheKey<SystemReleaseNoteDTO>($"CommonService-System-Release-Info-{id}");
            _cache.RemoveByCacheKey<SystemReleaseNoteDTO>("CommonService-System-Release-Info-Latest");
            await GetLatestReleaseInfo();
            await GetReleaseInfo(id);
        }

        #endregion
    }
}
