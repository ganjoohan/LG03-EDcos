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
    public class ProcedureStatusCacheRepository : IProcedureStatusCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IProcedureStatusRepository _procedurestatusRepository;

        public ProcedureStatusCacheRepository(IDistributedCache distributedCache, IProcedureStatusRepository procedurestatusRepository)
        {
            _distributedCache = distributedCache;
            _procedurestatusRepository = procedurestatusRepository;
        }

        public async Task<ProcedureStatus> GetByIdAsync(int procedureId)
        {
            string cacheKey = ProcedureStatusCacheKeys.GetKey(procedureId);
            var procedurestatus = await _distributedCache.GetAsync<ProcedureStatus>(cacheKey);
            if (procedurestatus == null)
            {
                procedurestatus = await _procedurestatusRepository.GetByIdAsync(procedureId);
                Throw.Exception.IfNull(procedurestatus, "PROCEDURESTATUS", "No PROCEDURESTATUS Found");
                await _distributedCache.SetAsync(cacheKey, procedurestatus);
            }
            return procedurestatus;
        }

        public async Task<List<ProcedureStatus>> GetCachedListAsync()
        {
            string cacheKey = ProcedureStatusCacheKeys.ListKey;
            var procedurestatusList = await _distributedCache.GetAsync<List<ProcedureStatus>>(cacheKey);
            if (procedurestatusList == null)
            {
                procedurestatusList = await _procedurestatusRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, procedurestatusList);
            }
            return procedurestatusList;
        }
    }


}
