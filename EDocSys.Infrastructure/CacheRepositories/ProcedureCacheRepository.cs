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
    public class ProcedureCacheRepository : IProcedureCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IProcedureRepository _procedureRepository;

        public ProcedureCacheRepository(IDistributedCache distributedCache, IProcedureRepository procedureRepository)
        {
            _distributedCache = distributedCache;
            _procedureRepository = procedureRepository;
        }

        public async Task<Procedure> GetByIdAsync(int procedureId)
        {
            string cacheKey = ProcedureCacheKeys.GetKey(procedureId);
            var procedure = await _distributedCache.GetAsync<Procedure>(cacheKey);
            if (procedure == null)
            {
                procedure = await _procedureRepository.GetByIdAsync(procedureId);
                Throw.Exception.IfNull(procedure, "Procedure", "No Procedure Found");
                await _distributedCache.SetAsync(cacheKey, procedure);
            }
            return procedure;
        }

        public async Task<List<Procedure>> GetByParameterAsync(int companyId, int departmentId)
        {
            string cacheKey = ProcedureCacheKeys.GetKeyParameter(companyId, departmentId);
            var procedureList = await _distributedCache.GetAsync<List<Procedure>>(cacheKey);
            if (procedureList == null)
            {
                procedureList = await _procedureRepository.GetByParameterAsync(companyId, departmentId);
                await _distributedCache.SetAsync(cacheKey, procedureList);
            }
            return procedureList;
        }

        public async Task<List<Procedure>> GetCachedListAsync()
        {
            string cacheKey = ProcedureCacheKeys.ListKey;
            var procedureList = await _distributedCache.GetAsync<List<Procedure>>(cacheKey);
            if (procedureList == null)
            {
                procedureList = await _procedureRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, procedureList);
            }
            return procedureList;
        }

        public async Task<Procedure> GetByWSCPNoAsync(string wscpno)
        {
            string cacheKey = ProcedureCacheKeys.GetKeyWSCPNo(wscpno);
            var procedure = await _distributedCache.GetAsync<Procedure>(cacheKey);
            if (procedure == null)
            {
                procedure = await _procedureRepository.GetByWSCPNoAsync(wscpno);
                Throw.Exception.IfNull(procedure, "Procedure", "No Procedure Found");
                await _distributedCache.SetAsync(cacheKey, procedure);
            }
            return procedure;
        }

    }
}