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
    public class EnvironmentalManualCacheRepository : IEnvironmentalManualCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IEnvironmentalManualRepository _environmentalManualRepository;

        public EnvironmentalManualCacheRepository(IDistributedCache distributedCache, IEnvironmentalManualRepository environmentalManualRepository)
        {
            _distributedCache = distributedCache;
            _environmentalManualRepository = environmentalManualRepository;
        }

        public async Task<EnvironmentalManual> GetByIdAsync(int environmentalManualId)
        {
            string cacheKey = EnvironmentalManualCacheKeys.GetKey(environmentalManualId);
            var environmentalManual = await _distributedCache.GetAsync<EnvironmentalManual>(cacheKey);
            if (environmentalManual == null)
            {
                environmentalManual = await _environmentalManualRepository.GetByIdAsync(environmentalManualId);
                Throw.Exception.IfNull(environmentalManual, "Environmental Manual", "No Environmental Manual Found");
                await _distributedCache.SetAsync(cacheKey, environmentalManual);
            }
            return environmentalManual;
        }

        public async Task<List<EnvironmentalManual>> GetCachedListAsync()
        {
            string cacheKey = EnvironmentalManualCacheKeys.ListKey;
            var environmentalManualList = await _distributedCache.GetAsync<List<EnvironmentalManual>>(cacheKey);
            if (environmentalManualList == null)
            {
                environmentalManualList = await _environmentalManualRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, environmentalManualList);
            }
            return environmentalManualList;
        }

        public async Task<EnvironmentalManual> GetByDOCNoAsync(string docno)
        {
            string cacheKey = EnvironmentalManualCacheKeys.GetKeyDOCNo(docno);
            var environmentalManual = await _distributedCache.GetAsync<EnvironmentalManual>(cacheKey);
            if (environmentalManual == null)
            {
                environmentalManual = await _environmentalManualRepository.GetByDOCNoAsync(docno);
                Throw.Exception.IfNull(environmentalManual, "Environmental Manual", "No Environmental Manual Found");
                await _distributedCache.SetAsync(cacheKey, environmentalManual);
            }
            return environmentalManual;
        }

    }
}