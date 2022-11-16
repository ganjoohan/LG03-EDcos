using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class SafetyHealthManualRepository : ISafetyHealthManualRepository
    {
        private readonly IRepositoryAsync<SafetyHealthManual> _repository;
        private readonly IDistributedCache _distributedCache;

        public SafetyHealthManualRepository(IDistributedCache distributedCache, IRepositoryAsync<SafetyHealthManual> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<SafetyHealthManual> SafetyHealthManuals => _repository.Entities;

        public async Task DeleteAsync(SafetyHealthManual safetyHealthManual)
        {
            await _repository.DeleteAsync(safetyHealthManual);
            await _distributedCache.RemoveAsync(CacheKeys.SafetyHealthManualCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.SafetyHealthManualCacheKeys.GetKey(safetyHealthManual.Id));
        }

        public async Task<SafetyHealthManual> GetByIdAsync(int safetyHealthManualId)
        {
            return await _repository.Entities.Where(p => p.Id == safetyHealthManualId)
                .Include(a => a.Company)
                .FirstOrDefaultAsync();
        }

        public async Task<SafetyHealthManual> GetByDOCNoAsync(string docno)
        {
            return await _repository.Entities.Where(p => p.DOCNo == docno)
                .FirstOrDefaultAsync();
        }


        public async Task<List<SafetyHealthManual>> GetListAsync()
        {
            return _repository.Entities
                .Include(a => a.Company)
                .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(SafetyHealthManual safetyHealthManual)
        {
            await _repository.AddAsync(safetyHealthManual);
            await _distributedCache.RemoveAsync(CacheKeys.SafetyHealthManualCacheKeys.ListKey);
            return safetyHealthManual.Id;
        }

        public async Task UpdateAsync(SafetyHealthManual safetyHealthManual)
        {
            await _repository.UpdateAsync(safetyHealthManual);
            await _distributedCache.RemoveAsync(CacheKeys.SafetyHealthManualCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.SafetyHealthManualCacheKeys.GetKey(safetyHealthManual.Id));
        }
    }
}