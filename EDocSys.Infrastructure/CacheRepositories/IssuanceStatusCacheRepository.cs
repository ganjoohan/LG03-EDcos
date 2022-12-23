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
    public class IssuanceStatusCacheRepository : IIssuanceStatusCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IIssuanceStatusRepository _issuanceStatusRepository;

        public IssuanceStatusCacheRepository(IDistributedCache distributedCache, IIssuanceStatusRepository issuanceStatusRepository)
        {
            _distributedCache = distributedCache;
            _issuanceStatusRepository = issuanceStatusRepository;
        }

        public async Task<IssuanceStatus> GetByIdAsync(int docId)
        {
            string cacheKey = IssuanceStatusCacheKeys.GetKey(docId);
            var issuancestatus = await _distributedCache.GetAsync<IssuanceStatus>(cacheKey);
            if (issuancestatus == null)
            {
                issuancestatus = await _issuanceStatusRepository.GetByIdAsync(docId);
                Throw.Exception.IfNull(issuancestatus, "Issuance Status", "No Issuance Status Found");
                await _distributedCache.SetAsync(cacheKey, issuancestatus);
            }
            return issuancestatus;
        }

        public async Task<List<IssuanceStatus>> GetCachedListAsync()
        {
            string cacheKey = IssuanceStatusCacheKeys.ListKey;
            var issuanceStatusList = await _distributedCache.GetAsync<List<IssuanceStatus>>(cacheKey);
            if (issuanceStatusList == null)
            {
                issuanceStatusList = await _issuanceStatusRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, issuanceStatusList);
            }
            return issuanceStatusList;
        }
    }


}
