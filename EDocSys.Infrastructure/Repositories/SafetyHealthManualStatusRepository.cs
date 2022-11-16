using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class SafetyHealthManualStatusRepository : ISafetyHealthManualStatusRepository
    {
        private readonly IRepositoryAsync<SafetyHealthManualStatus> _repository;
        private readonly IDistributedCache _distributedCache;

        public SafetyHealthManualStatusRepository(IDistributedCache distributedCache, IRepositoryAsync<SafetyHealthManualStatus> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<SafetyHealthManualStatus> SafetyHealthManualStatuses => _repository.Entities;

        public Task DeleteAsync(SafetyHealthManualStatus safetyHealthManualstatusId)
        {
            throw new System.NotImplementedException();
        }

        //public async Task DeleteAsync(SafetyHealthManual SafetyHealthManual)
        //{
        //    await _repository.DeleteAsync(SafetyHealthManual);
        //    await _distributedCache.RemoveAsync(CacheKeys.SafetyHealthManualCacheKeys.ListKey);
        //    await _distributedCache.RemoveAsync(CacheKeys.SafetyHealthManualCacheKeys.GetKey(SafetyHealthManual.Id));
        //}

        public async Task<SafetyHealthManualStatus> GetByIdAsync(int safetyHealthManualId)
        {
            return await _repository.Entities.Where(p => p.SafetyHealthManual.Id == safetyHealthManualId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<SafetyHealthManualStatus>> GetListAsync()
        {
            return await _repository.Entities
                //.Include(a => a.SafetyHealthManual)
                .Include(a => a.DocumentStatus)
                .ToListAsync();
        }

        public Task<List<SafetyHealthManualStatus>> GetListByIdAsync(int safetyHealthManualId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> InsertAsync(SafetyHealthManualStatus safetyHealthManualstatusId)
        {
            await _repository.AddAsync(safetyHealthManualstatusId);
            await _distributedCache.RemoveAsync(CacheKeys.SafetyHealthManualStatusCacheKeys.ListKey);
            return safetyHealthManualstatusId.Id;
        }

        public Task UpdateAsync(SafetyHealthManualStatus safetyHealthManualstatusId)
        {
            throw new System.NotImplementedException();
        }
    }
}