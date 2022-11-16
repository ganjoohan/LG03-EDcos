using EDocSys.Application.Interfaces.CacheRepositories;
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Infrastructure.CacheKeys;
using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.CacheRepositories
{
    public class SafetyHealthManualStatusCacheRepository : ISafetyHealthManualStatusCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ISafetyHealthManualStatusRepository _safetyHealthManualstatusRepository;

        public SafetyHealthManualStatusCacheRepository(IDistributedCache distributedCache, ISafetyHealthManualStatusRepository safetyHealthManualstatusRepository)
        {
            _distributedCache = distributedCache;
            _safetyHealthManualstatusRepository = safetyHealthManualstatusRepository;
        }

        public async Task<SafetyHealthManualStatus> GetByIdAsync(int safetyHealthManualId)
        {
            string cacheKey = SafetyHealthManualStatusCacheKeys.GetKey(safetyHealthManualId);
            var safetyHealthManualstatus = await _distributedCache.GetAsync<SafetyHealthManualStatus>(cacheKey);
            if (safetyHealthManualstatus == null)
            {
                safetyHealthManualstatus = await _safetyHealthManualstatusRepository.GetByIdAsync(safetyHealthManualId);
                Throw.Exception.IfNull(safetyHealthManualstatus, "Safety and Health Manual Status", "No Safety and Health Manual Status Found");
                await _distributedCache.SetAsync(cacheKey, safetyHealthManualstatus);
            }
            return safetyHealthManualstatus;
        }

        public async Task<List<SafetyHealthManualStatus>> GetCachedListAsync()
        {
            string cacheKey = SafetyHealthManualStatusCacheKeys.ListKey;
            var safetyHealthManualstatusList = await _distributedCache.GetAsync<List<SafetyHealthManualStatus>>(cacheKey);
            if (safetyHealthManualstatusList == null)
            {
                safetyHealthManualstatusList = await _safetyHealthManualstatusRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, safetyHealthManualstatusList);
            }
            return safetyHealthManualstatusList;
        }
    }


}
