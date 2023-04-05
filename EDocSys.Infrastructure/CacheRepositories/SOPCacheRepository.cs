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
    public class SOPCacheRepository : ISOPCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ISOPRepository _sopRepository;

        public SOPCacheRepository(IDistributedCache distributedCache, ISOPRepository sopRepository)
        {
            _distributedCache = distributedCache;
            _sopRepository = sopRepository;
        }

        public async Task<SOP> GetByIdAsync(int sopId)
        {
            string cacheKey = SOPCacheKeys.GetKey(sopId);
            var sop = await _distributedCache.GetAsync<SOP>(cacheKey);
            if (sop == null)
            {
                sop = await _sopRepository.GetByIdAsync(sopId);
                Throw.Exception.IfNull(sop, "SOP", "No SOP Found");
                await _distributedCache.SetAsync(cacheKey, sop);
            }
            return sop;
        }
        public async Task<List<SOP>> GetByParameterAsync(int companyId, int departmentId)
        {
            string cacheKey = SOPCacheKeys.GetKeyParameter(companyId, departmentId);
            var sopList = await _distributedCache.GetAsync<List<SOP>>(cacheKey);
            if (sopList == null)
            {
                sopList = await _sopRepository.GetByParameterAsync(companyId, departmentId);
                await _distributedCache.SetAsync(cacheKey, sopList);
            }
            return sopList;
        }
        public async Task<List<SOP>> GetCachedListAsync()
        {
            string cacheKey = SOPCacheKeys.ListKey;
            var sopList = await _distributedCache.GetAsync<List<SOP>>(cacheKey);
            if (sopList == null)
            {
                sopList = await _sopRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, sopList);
            }
            return sopList;
        }
    }
}