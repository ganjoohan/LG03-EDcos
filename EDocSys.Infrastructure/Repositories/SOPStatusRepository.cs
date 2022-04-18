using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class SOPStatusRepository : ISOPStatusRepository
    {
        private readonly IRepositoryAsync<SOPStatus> _repository;
        private readonly IDistributedCache _distributedCache;

        public SOPStatusRepository(IDistributedCache distributedCache, IRepositoryAsync<SOPStatus> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<SOPStatus> SOPStatus => _repository.Entities;

        public Task DeleteAsync(SOPStatus sopstatusId)
        {
            throw new System.NotImplementedException();
        }

        //public async Task DeleteAsync(Procedure procedure)
        //{
        //    await _repository.DeleteAsync(procedure);
        //    await _distributedCache.RemoveAsync(CacheKeys.ProcedureCacheKeys.ListKey);
        //    await _distributedCache.RemoveAsync(CacheKeys.ProcedureCacheKeys.GetKey(procedure.Id));
        //}

        public async Task<SOPStatus> GetByIdAsync(int sopId)
        {
            return await _repository.Entities.Where(p => p.Sop.Id == sopId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<SOPStatus>> GetListAsync()
        {
            return await _repository.Entities
                //.Include(a => a.Procedure)
                .Include(a => a.DocumentStatus)
                .ToListAsync();
        }

        public Task<List<SOPStatus>> GetListByIdAsync(int sopId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> InsertAsync(SOPStatus sopstatusId)
        {
            await _repository.AddAsync(sopstatusId);
            await _distributedCache.RemoveAsync(CacheKeys.SOPStatusCacheKeys.ListKey);
            return sopstatusId.Id;
        }

        public Task UpdateAsync(SOPStatus sopstatusId)
        {
            throw new System.NotImplementedException();
        }
    }
}