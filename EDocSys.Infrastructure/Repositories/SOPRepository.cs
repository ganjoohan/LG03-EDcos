using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class SOPRepository : ISOPRepository
    {
        private readonly IRepositoryAsync<SOP> _repository;
        private readonly IDistributedCache _distributedCache;

        public SOPRepository(IDistributedCache distributedCache, IRepositoryAsync<SOP> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<SOP> SOPs => _repository.Entities;

        public async Task DeleteAsync(SOP sop)
        {
            await _repository.DeleteAsync(sop);
            await _distributedCache.RemoveAsync(CacheKeys.SOPCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.SOPCacheKeys.GetKey(sop.Id));
        }

        public async Task<SOP> GetByIdAsync(int sopId)
        {
            return await _repository.Entities.Where(p => p.Id == sopId)
                .Include(a => a.Department)
                .Include(a => a.Company)
                .FirstOrDefaultAsync();
        }

        public async Task<SOP> GetByWSCPNoAsync(string wscpno)
        {
            return await _repository.Entities.Where(p => p.WSCPNo == wscpno)
                .FirstOrDefaultAsync();
        }


        public async Task<List<SOP>> GetListAsync()
        {
            return _repository.Entities
               .Include(a => a.Department)
               .Include(a => a.Company)
               .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Department)
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(SOP sop)
        {
            await _repository.AddAsync(sop);
            await _distributedCache.RemoveAsync(CacheKeys.SOPCacheKeys.ListKey);
            return sop.Id;
        }

        public async Task UpdateAsync(SOP sop)
        {
            await _repository.UpdateAsync(sop);
            await _distributedCache.RemoveAsync(CacheKeys.SOPCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.SOPCacheKeys.GetKey(sop.Id));
        }
    }
}