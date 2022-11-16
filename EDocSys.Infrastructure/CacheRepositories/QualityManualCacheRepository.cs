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
    public class QualityManualCacheRepository : IQualityManualCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IQualityManualRepository _qualityManualRepository;

        public QualityManualCacheRepository(IDistributedCache distributedCache, IQualityManualRepository qualityManualRepository)
        {
            _distributedCache = distributedCache;
            _qualityManualRepository = qualityManualRepository;
        }

        public async Task<QualityManual> GetByIdAsync(int qualityManualId)
        {
            string cacheKey = QualityManualCacheKeys.GetKey(qualityManualId);
            var qualityManual = await _distributedCache.GetAsync<QualityManual>(cacheKey);
            if (qualityManual == null)
            {
                qualityManual = await _qualityManualRepository.GetByIdAsync(qualityManualId);
                Throw.Exception.IfNull(qualityManual, "Quality Manual", "No Quality Manual Found");
                await _distributedCache.SetAsync(cacheKey, qualityManual);
            }
            return qualityManual;
        }

        public async Task<List<QualityManual>> GetCachedListAsync()
        {
            string cacheKey = QualityManualCacheKeys.ListKey;
            var qualityManualList = await _distributedCache.GetAsync<List<QualityManual>>(cacheKey);
            if (qualityManualList == null)
            {
                qualityManualList = await _qualityManualRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, qualityManualList);
            }
            return qualityManualList;
        }

        public async Task<QualityManual> GetByDOCNoAsync(string docno)
        {
            string cacheKey = QualityManualCacheKeys.GetKeyDOCNo(docno);
            var qualityManual = await _distributedCache.GetAsync<QualityManual>(cacheKey);
            if (qualityManual == null)
            {
                qualityManual = await _qualityManualRepository.GetByDOCNoAsync(docno);
                Throw.Exception.IfNull(qualityManual, "Quality Manual", "No Quality Manual Found");
                await _distributedCache.SetAsync(cacheKey, qualityManual);
            }
            return qualityManual;
        }

    }
}