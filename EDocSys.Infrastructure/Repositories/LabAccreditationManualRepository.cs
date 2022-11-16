using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class LabAccreditationManualRepository : ILabAccreditationManualRepository
    {
        private readonly IRepositoryAsync<LabAccreditationManual> _repository;
        private readonly IDistributedCache _distributedCache;

        public LabAccreditationManualRepository(IDistributedCache distributedCache, IRepositoryAsync<LabAccreditationManual> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<LabAccreditationManual> LabAccreditationManuals => _repository.Entities;

        public async Task DeleteAsync(LabAccreditationManual labAccreditationManual)
        {
            await _repository.DeleteAsync(labAccreditationManual);
            await _distributedCache.RemoveAsync(CacheKeys.LabAccreditationManualCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.LabAccreditationManualCacheKeys.GetKey(labAccreditationManual.Id));
        }

        public async Task<LabAccreditationManual> GetByIdAsync(int labAccreditationManualId)
        {
            return await _repository.Entities.Where(p => p.Id == labAccreditationManualId)
                .Include(a => a.Company)
                .FirstOrDefaultAsync();
        }

        public async Task<LabAccreditationManual> GetByDOCNoAsync(string docno)
        {
            return await _repository.Entities.Where(p => p.DOCNo == docno)
                .FirstOrDefaultAsync();
        }


        public async Task<List<LabAccreditationManual>> GetListAsync()
        {
            return _repository.Entities
               .Include(a => a.Company)
               .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(LabAccreditationManual labAccreditationManual)
        {
            await _repository.AddAsync(labAccreditationManual);
            await _distributedCache.RemoveAsync(CacheKeys.LabAccreditationManualCacheKeys.ListKey);
            return labAccreditationManual.Id;
        }

        public async Task UpdateAsync(LabAccreditationManual labAccreditationManual)
        {
            await _repository.UpdateAsync(labAccreditationManual);
            await _distributedCache.RemoveAsync(CacheKeys.LabAccreditationManualCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.LabAccreditationManualCacheKeys.GetKey(labAccreditationManual.Id));
        }
    }
}