using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class QualityManualRepository : IQualityManualRepository
    {
        private readonly IRepositoryAsync<QualityManual> _repository;
        private readonly IDistributedCache _distributedCache;

        public QualityManualRepository(IDistributedCache distributedCache, IRepositoryAsync<QualityManual> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<QualityManual> QualityManuals => _repository.Entities;

        public async Task DeleteAsync(QualityManual QualityManual)
        {
            await _repository.DeleteAsync(QualityManual);
            await _distributedCache.RemoveAsync(CacheKeys.QualityManualCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.QualityManualCacheKeys.GetKey(QualityManual.Id));
        }

        public async Task<QualityManual> GetByIdAsync(int qualityManualId)
        {
            return await _repository.Entities.Where(p => p.Id == qualityManualId)
                .Include(a => a.Company)
                .FirstOrDefaultAsync();
        }

        public async Task<QualityManual> GetByDOCNoAsync(string docno)
        {
            return await _repository.Entities.Where(p => p.DOCNo == docno)
                .FirstOrDefaultAsync();
        }


        public async Task<List<QualityManual>> GetListAsync()
        {
            return _repository.Entities
               .Include(a => a.Company)
               .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(QualityManual qualityManual)
        {
            await _repository.AddAsync(qualityManual);
            await _distributedCache.RemoveAsync(CacheKeys.QualityManualCacheKeys.ListKey);
            return qualityManual.Id;
        }

        public async Task UpdateAsync(QualityManual qualityManual)
        {
            await _repository.UpdateAsync(qualityManual);
            await _distributedCache.RemoveAsync(CacheKeys.QualityManualCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.QualityManualCacheKeys.GetKey(qualityManual.Id));
        }
    }
}