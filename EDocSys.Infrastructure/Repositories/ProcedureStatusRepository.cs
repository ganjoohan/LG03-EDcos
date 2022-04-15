using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class ProcedureStatusRepository : IProcedureStatusRepository
    {
        private readonly IRepositoryAsync<ProcedureStatus> _repository;
        private readonly IDistributedCache _distributedCache;

        public ProcedureStatusRepository(IDistributedCache distributedCache, IRepositoryAsync<ProcedureStatus> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<ProcedureStatus> ProcedureStatus => _repository.Entities;

        public Task DeleteAsync(ProcedureStatus procedurestatusId)
        {
            throw new System.NotImplementedException();
        }

        //public async Task DeleteAsync(Procedure procedure)
        //{
        //    await _repository.DeleteAsync(procedure);
        //    await _distributedCache.RemoveAsync(CacheKeys.ProcedureCacheKeys.ListKey);
        //    await _distributedCache.RemoveAsync(CacheKeys.ProcedureCacheKeys.GetKey(procedure.Id));
        //}

        public async Task<ProcedureStatus> GetByIdAsync(int procedureId)
        {
            return await _repository.Entities.Where(p => p.Procedure.Id == procedureId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ProcedureStatus>> GetListAsync()
        {
            return await _repository.Entities
                //.Include(a => a.Procedure)
                .Include(a => a.DocumentStatus)
                .ToListAsync();
        }

        public Task<List<ProcedureStatus>> GetListByIdAsync(int procedureId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> InsertAsync(ProcedureStatus procedurestatusId)
        {
            await _repository.AddAsync(procedurestatusId);
            await _distributedCache.RemoveAsync(CacheKeys.ProcedureStatusCacheKeys.ListKey);
            return procedurestatusId.Id;
        }

        public Task UpdateAsync(ProcedureStatus procedurestatusId)
        {
            throw new System.NotImplementedException();
        }
    }
}