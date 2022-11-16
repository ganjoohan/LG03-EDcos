using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class EnvironmentalManualRepository : IEnvironmentalManualRepository
    {
        private readonly IRepositoryAsync<EnvironmentalManual> _repository;
        private readonly IDistributedCache _distributedCache;

        public EnvironmentalManualRepository(IDistributedCache distributedCache, IRepositoryAsync<EnvironmentalManual> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<EnvironmentalManual> EnvironmentalManuals => _repository.Entities;

        public async Task DeleteAsync(EnvironmentalManual EnvironmentalManual)
        {
            await _repository.DeleteAsync(EnvironmentalManual);
            await _distributedCache.RemoveAsync(CacheKeys.EnvironmentalManualCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.EnvironmentalManualCacheKeys.GetKey(EnvironmentalManual.Id));
        }

        public async Task<EnvironmentalManual> GetByIdAsync(int EnvironmentalManualId)
        {
            return await _repository.Entities.Where(p => p.Id == EnvironmentalManualId)
                .Include(a => a.Company)
                .FirstOrDefaultAsync();
        }

        public async Task<EnvironmentalManual> GetByDOCNoAsync(string docno)
        {
            return await _repository.Entities.Where(p => p.DOCNo == docno)
                .FirstOrDefaultAsync();
        }


        public async Task<List<EnvironmentalManual>> GetListAsync()
        {
            return _repository.Entities
               .Include(a => a.Company)
               .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(EnvironmentalManual EnvironmentalManual)
        {
            await _repository.AddAsync(EnvironmentalManual);
            await _distributedCache.RemoveAsync(CacheKeys.EnvironmentalManualCacheKeys.ListKey);
            return EnvironmentalManual.Id;
        }

        public async Task UpdateAsync(EnvironmentalManual EnvironmentalManual)
        {
            await _repository.UpdateAsync(EnvironmentalManual);
            await _distributedCache.RemoveAsync(CacheKeys.EnvironmentalManualCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.EnvironmentalManualCacheKeys.GetKey(EnvironmentalManual.Id));
        }
    }
}