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
    public class IssuanceCacheRepository : IIssuanceCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IIssuanceRepository _issuanceRepository;

        public IssuanceCacheRepository(IDistributedCache distributedCache, IIssuanceRepository issuanceRepository)
        {
            _distributedCache = distributedCache;
            _issuanceRepository = issuanceRepository;
        }

        public async Task<Issuance> GetByIdAsync(int docId)
        {
            string cacheKey = IssuanceCacheKeys.GetKey(docId);
            var issuance = await _distributedCache.GetAsync<Issuance>(cacheKey);
            if (issuance == null)
            {
                issuance = await _issuanceRepository.GetByIdAsync(docId);
                Throw.Exception.IfNull(issuance, "Issuance", "No Issuance Found");
                await _distributedCache.SetAsync(cacheKey, issuance);
            }
            return issuance;
        }

        public async Task<List<Issuance>> GetCachedListAsync()
        {
            string cacheKey = IssuanceCacheKeys.ListKey;
            var issuanceList = await _distributedCache.GetAsync<List<Issuance>>(cacheKey);
            if (issuanceList == null)
            {
                issuanceList = await _issuanceRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, issuanceList);
            }
            return issuanceList;
        }

        public async Task<Issuance> GetByDOCNoAsync(string docno)
        {
            string cacheKey = IssuanceCacheKeys.GetKeyDOCNo(docno);
            var issuance = await _distributedCache.GetAsync<Issuance>(cacheKey);
            if (issuance == null)
            {
                issuance = await _issuanceRepository.GetByDOCNoAsync(docno);
                Throw.Exception.IfNull(issuance, "Issuance", "No Issuance Found");
                await _distributedCache.SetAsync(cacheKey, issuance);
            }
            return issuance;
        }

    }
}