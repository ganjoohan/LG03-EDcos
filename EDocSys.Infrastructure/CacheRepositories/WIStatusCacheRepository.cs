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
    public class WIStatusCacheRepository : IWIStatusCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IWIStatusRepository _wistatusRepository;

        public WIStatusCacheRepository(IDistributedCache distributedCache, IWIStatusRepository wistatusRepository)
        {
            _distributedCache = distributedCache;
            _wistatusRepository = wistatusRepository;
        }

        public async Task<WIStatus> GetByIdAsync(int wiId)
        {
            string cacheKey = WIStatusCacheKeys.GetKey(wiId);
            var wistatus = await _distributedCache.GetAsync<WIStatus>(cacheKey);
            if (wistatus == null)
            {
                wistatus = await _wistatusRepository.GetByIdAsync(wiId);
                Throw.Exception.IfNull(wistatus, "PROCEDURESTATUS", "No PROCEDURESTATUS Found");
                await _distributedCache.SetAsync(cacheKey, wistatus);
            }
            return wistatus;
        }

        public async Task<List<WIStatus>> GetCachedListAsync()
        {
            string cacheKey = WIStatusCacheKeys.ListKey;
            var wistatusList = await _distributedCache.GetAsync<List<WIStatus>>(cacheKey);
            if (wistatusList == null)
            {
                wistatusList = await _wistatusRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, wistatusList);
            }
            return wistatusList;
        }
    }


}
