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
    public class EnvironmentalManualStatusCacheRepository : IEnvironmentalManualStatusCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IEnvironmentalManualStatusRepository _environmentalManualstatusRepository;

        public EnvironmentalManualStatusCacheRepository(IDistributedCache distributedCache, IEnvironmentalManualStatusRepository environmentalManualstatusRepository)
        {
            _distributedCache = distributedCache;
            _environmentalManualstatusRepository = environmentalManualstatusRepository;
        }

        public async Task<EnvironmentalManualStatus> GetByIdAsync(int environmentalManualId)
        {
            string cacheKey = EnvironmentalManualStatusCacheKeys.GetKey(environmentalManualId);
            var environmentalManualstatus = await _distributedCache.GetAsync<EnvironmentalManualStatus>(cacheKey);
            if (environmentalManualstatus == null)
            {
                environmentalManualstatus = await _environmentalManualstatusRepository.GetByIdAsync(environmentalManualId);
                Throw.Exception.IfNull(environmentalManualstatus, "Environmental Manual Status", "No Environmental Manual Status Found");
                await _distributedCache.SetAsync(cacheKey, environmentalManualstatus);
            }
            return environmentalManualstatus;
        }

        public async Task<List<EnvironmentalManualStatus>> GetCachedListAsync()
        {
            string cacheKey = EnvironmentalManualStatusCacheKeys.ListKey;
            var environmentalManualstatusList = await _distributedCache.GetAsync<List<EnvironmentalManualStatus>>(cacheKey);
            if (environmentalManualstatusList == null)
            {
                environmentalManualstatusList = await _environmentalManualstatusRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, environmentalManualstatusList);
            }
            return environmentalManualstatusList;
        }
    }


}
