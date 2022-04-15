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
    public class WICacheRepository : IWICacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IWIRepository _wiRepository;

        public WICacheRepository(IDistributedCache distributedCache, IWIRepository wiRepository)
        {
            _distributedCache = distributedCache;
            _wiRepository = wiRepository;
        }

        public async Task<WI> GetByIdAsync(int wiId)
        {
            string cacheKey = WICacheKeys.GetKey(wiId);
            var wi = await _distributedCache.GetAsync<WI>(cacheKey);
            if (wi == null)
            {
                wi = await _wiRepository.GetByIdAsync(wiId);
                Throw.Exception.IfNull(wi, "WI", "No WI Found");
                await _distributedCache.SetAsync(cacheKey, wi);
            }
            return wi;
        }

        public async Task<List<WI>> GetCachedListAsync()
        {
            string cacheKey = WICacheKeys.ListKey;
            var wiList = await _distributedCache.GetAsync<List<WI>>(cacheKey);
            if (wiList == null)
            {
                wiList = await _wiRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, wiList);
            }
            return wiList;
        }
    }
}