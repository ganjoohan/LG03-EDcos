using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Infrastructure.CacheKeys;
using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDocSys.Domain.Entities.DocumentationMaster;
using EDocSys.Application.Interfaces.CacheRepositories.ExternalCacheRepositories;
using EDocSys.Application.Interfaces.Repositories.ExternalRepositories;
using EDocSys.Domain.Entities.ExternalRecord;
using EDocSys.Infrastructure.CacheKeys.ExternalCacheKeys;

namespace EDocSys.Infrastructure.CacheRepositories.ExternalCacheRepositories
{
    public class LionSteelCacheRepository : ILionSteelCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILionSteelRepository _lionSteelRepository;

        public LionSteelCacheRepository(IDistributedCache distributedCache, ILionSteelRepository lionSteelRepository)
        {
            _distributedCache = distributedCache;
            _lionSteelRepository = lionSteelRepository;
        }

        public async Task<LionSteel> GetByDOCNoAsync(string docno)
        {
            string cacheKey = LionSteelCacheKeys.GetKeyDOCNo(docno);
            var lionSteel = await _distributedCache.GetAsync<LionSteel>(cacheKey);
            if (lionSteel == null)
            {
                lionSteel = await _lionSteelRepository.GetByDOCNoAsync(docno);
                Throw.Exception.IfNull(lionSteel, "LionSteel", "No LionSteel Found");
                await _distributedCache.SetAsync(cacheKey, lionSteel);
            }
            return lionSteel;
        }

        public async Task<LionSteel> GetByIdAsync(int lionSteelId)
        {
            string cacheKey = LionSteelCacheKeys.GetKey(lionSteelId);
            var lionSteel = await _distributedCache.GetAsync<LionSteel>(cacheKey);
            if (lionSteel == null)
            {
                lionSteel = await _lionSteelRepository.GetByIdAsync(lionSteelId);
                Throw.Exception.IfNull(lionSteel, "LionSteel", "No LionSteel Found");
                await _distributedCache.SetAsync(cacheKey, lionSteel);
            }
            return lionSteel;
        }

        public async Task<List<LionSteel>> GetCachedListAsync()
        {
            string cacheKey = LionSteelCacheKeys.ListKey;
            var lionSteelList = await _distributedCache.GetAsync<List<LionSteel>>(cacheKey);
            if (lionSteelList == null)
            {
                lionSteelList = await _lionSteelRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, lionSteelList);
            }
            return lionSteelList;
        }
    }
}
