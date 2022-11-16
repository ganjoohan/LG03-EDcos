using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class EnvironmentalManualStatusRepository : IEnvironmentalManualStatusRepository
    {
        private readonly IRepositoryAsync<EnvironmentalManualStatus> _repository;
        private readonly IDistributedCache _distributedCache;

        public EnvironmentalManualStatusRepository(IDistributedCache distributedCache, IRepositoryAsync<EnvironmentalManualStatus> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<EnvironmentalManualStatus> EnvironmentalManualStatuses => _repository.Entities;

        public Task DeleteAsync(EnvironmentalManualStatus EnvironmentalManualstatusId)
        {
            throw new System.NotImplementedException();
        }

        //public async Task DeleteAsync(EnvironmentalManual EnvironmentalManual)
        //{
        //    await _repository.DeleteAsync(EnvironmentalManual);
        //    await _distributedCache.RemoveAsync(CacheKeys.EnvironmentalManualCacheKeys.ListKey);
        //    await _distributedCache.RemoveAsync(CacheKeys.EnvironmentalManualCacheKeys.GetKey(EnvironmentalManual.Id));
        //}

        public async Task<EnvironmentalManualStatus> GetByIdAsync(int EnvironmentalManualId)
        {
            return await _repository.Entities.Where(p => p.EnvironmentalManual.Id == EnvironmentalManualId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<EnvironmentalManualStatus>> GetListAsync()
        {
            return await _repository.Entities
                //.Include(a => a.EnvironmentalManual)
                .Include(a => a.DocumentStatus)
                .ToListAsync();
        }

        public Task<List<EnvironmentalManualStatus>> GetListByIdAsync(int EnvironmentalManualId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> InsertAsync(EnvironmentalManualStatus EnvironmentalManualstatusId)
        {
            await _repository.AddAsync(EnvironmentalManualstatusId);
            await _distributedCache.RemoveAsync(CacheKeys.EnvironmentalManualStatusCacheKeys.ListKey);
            return EnvironmentalManualstatusId.Id;
        }

        public Task UpdateAsync(EnvironmentalManualStatus EnvironmentalManualstatusId)
        {
            throw new System.NotImplementedException();
        }
    }
}