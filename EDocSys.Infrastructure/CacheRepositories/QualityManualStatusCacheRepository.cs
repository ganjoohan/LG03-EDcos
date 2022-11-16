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
    public class QualityManualStatusCacheRepository : IQualityManualStatusCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQualityManualStatusRepository _qualityManualstatusRepository;

        public QualityManualStatusCacheRepository(IDistributedCache distributedCache, IQualityManualStatusRepository qualityManualstatusRepository)
        {
            _distributedCache = distributedCache;
            _qualityManualstatusRepository = qualityManualstatusRepository;
        }

        public async Task<QualityManualStatus> GetByIdAsync(int qualityManualId)
        {
            string cacheKey = QualityManualStatusCacheKeys.GetKey(qualityManualId);
            var qualityManualstatus = await _distributedCache.GetAsync<QualityManualStatus>(cacheKey);
            if (qualityManualstatus == null)
            {
                qualityManualstatus = await _qualityManualstatusRepository.GetByIdAsync(qualityManualId);
                Throw.Exception.IfNull(qualityManualstatus, "Quality Manual Status", "No Quality Manual Status Found");
                await _distributedCache.SetAsync(cacheKey, qualityManualstatus);
            }
            return qualityManualstatus;
        }

        public async Task<List<QualityManualStatus>> GetCachedListAsync()
        {
            string cacheKey = QualityManualStatusCacheKeys.ListKey;
            var qualityManualstatusList = await _distributedCache.GetAsync<List<QualityManualStatus>>(cacheKey);
            if (qualityManualstatusList == null)
            {
                qualityManualstatusList = await _qualityManualstatusRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, qualityManualstatusList);
            }
            return qualityManualstatusList;
        }
    }


}
