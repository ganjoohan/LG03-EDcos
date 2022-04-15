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
    public class SOPStatusCacheRepository : ISOPStatusCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ISOPStatusRepository _sopstatusRepository;

        public SOPStatusCacheRepository(IDistributedCache distributedCache, ISOPStatusRepository sopstatusRepository)
        {
            _distributedCache = distributedCache;
            _sopstatusRepository = sopstatusRepository;
        }

        public async Task<SOPStatus> GetByIdAsync(int sopId)
        {
            string cacheKey = SOPStatusCacheKeys.GetKey(sopId);
            var sopstatus = await _distributedCache.GetAsync<SOPStatus>(cacheKey);
            if (sopstatus == null)
            {
                sopstatus = await _sopstatusRepository.GetByIdAsync(sopId);
                Throw.Exception.IfNull(sopstatus, "SOPSTATUS", "No SOPSTATUS Found");
                await _distributedCache.SetAsync(cacheKey, sopstatus);
            }
            return sopstatus;
        }

        public async Task<List<SOPStatus>> GetCachedListAsync()
        {
            string cacheKey = SOPStatusCacheKeys.ListKey;
            var sopstatusList = await _distributedCache.GetAsync<List<SOPStatus>>(cacheKey);
            if (sopstatusList == null)
            {
                sopstatusList = await _sopstatusRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, sopstatusList);
            }
            return sopstatusList;
        }
    }


}
