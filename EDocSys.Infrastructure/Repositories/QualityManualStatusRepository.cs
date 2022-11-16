using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class QualityManualStatusRepository : IQualityManualStatusRepository
    {
        private readonly IRepositoryAsync<QualityManualStatus> _repository;
        private readonly IDistributedCache _distributedCache;

        public QualityManualStatusRepository(IDistributedCache distributedCache, IRepositoryAsync<QualityManualStatus> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<QualityManualStatus> QualityManualStatuses => _repository.Entities;

        public Task DeleteAsync(QualityManualStatus qualityManualstatusId)
        {
            throw new System.NotImplementedException();
        }

        //public async Task DeleteAsync(QualityManual qualityManual)
        //{
        //    await _repository.DeleteAsync(qualityManual);
        //    await _distributedCache.RemoveAsync(CacheKeys.QualityManualCacheKeys.ListKey);
        //    await _distributedCache.RemoveAsync(CacheKeys.QualityManualCacheKeys.GetKey(qualityManual.Id));
        //}

        public async Task<QualityManualStatus> GetByIdAsync(int qualityManualId)
        {
            return await _repository.Entities.Where(p => p.QualityManual.Id == qualityManualId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<QualityManualStatus>> GetListAsync()
        {
            return await _repository.Entities
                //.Include(a => a.QualityManual)
                .Include(a => a.DocumentStatus)
                .ToListAsync();
        }

        public Task<List<QualityManualStatus>> GetListByIdAsync(int qualityManualId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> InsertAsync(QualityManualStatus qualityManualstatusId)
        {
            await _repository.AddAsync(qualityManualstatusId);
            await _distributedCache.RemoveAsync(CacheKeys.QualityManualStatusCacheKeys.ListKey);
            return qualityManualstatusId.Id;
        }

        public Task UpdateAsync(QualityManualStatus qualityManualstatusId)
        {
            throw new System.NotImplementedException();
        }
    }
}