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
    public class SafetyHealthManualCacheRepository : ISafetyHealthManualCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ISafetyHealthManualRepository _safetyHealthManualRepository;

        public SafetyHealthManualCacheRepository(IDistributedCache distributedCache, ISafetyHealthManualRepository safetyHealthManualRepository)
        {
            _distributedCache = distributedCache;
            _safetyHealthManualRepository = safetyHealthManualRepository;
        }

        public async Task<SafetyHealthManual> GetByIdAsync(int safetyHealthManualId)
        {
            string cacheKey = SafetyHealthManualCacheKeys.GetKey(safetyHealthManualId);
            var safetyHealthManual = await _distributedCache.GetAsync<SafetyHealthManual>(cacheKey);
            if (safetyHealthManual == null)
            {
                safetyHealthManual = await _safetyHealthManualRepository.GetByIdAsync(safetyHealthManualId);
                Throw.Exception.IfNull(safetyHealthManual, "Safety and Health Manual", "No Safety and Health Manual Found");
                await _distributedCache.SetAsync(cacheKey, safetyHealthManual);
            }
            return safetyHealthManual;
        }

        public async Task<List<SafetyHealthManual>> GetCachedListAsync()
        {
            string cacheKey = SafetyHealthManualCacheKeys.ListKey;
            var safetyHealthManualList = await _distributedCache.GetAsync<List<SafetyHealthManual>>(cacheKey);
            if (safetyHealthManualList == null)
            {
                safetyHealthManualList = await _safetyHealthManualRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, safetyHealthManualList);
            }
            return safetyHealthManualList;
        }

        public async Task<SafetyHealthManual> GetByDOCNoAsync(string docno)
        {
            string cacheKey = SafetyHealthManualCacheKeys.GetKeyDOCNo(docno);
            var safetyHealthManual = await _distributedCache.GetAsync<SafetyHealthManual>(cacheKey);
            if (safetyHealthManual == null)
            {
                safetyHealthManual = await _safetyHealthManualRepository.GetByDOCNoAsync(docno);
                Throw.Exception.IfNull(safetyHealthManual, "Safety and Health Manual", "No Safety and Health Manual Found");
                await _distributedCache.SetAsync(cacheKey, safetyHealthManual);
            }
            return safetyHealthManual;
        }

    }
}