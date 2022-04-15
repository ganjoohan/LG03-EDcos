using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class WIRepository : IWIRepository
    {
        private readonly IRepositoryAsync<WI> _repository;
        private readonly IDistributedCache _distributedCache;

        public WIRepository(IDistributedCache distributedCache, IRepositoryAsync<WI> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<WI> WIs => _repository.Entities;

        public async Task DeleteAsync(WI wi)
        {
            await _repository.DeleteAsync(wi);
            await _distributedCache.RemoveAsync(CacheKeys.WICacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.WICacheKeys.GetKey(wi.Id));
        }

        public async Task<WI> GetByIdAsync(int wiId)
        {
            return await _repository.Entities.Where(p => p.Id == wiId)
                .Include(a => a.Department)
                .Include(a => a.Company)
                .FirstOrDefaultAsync();
        }

        public async Task<List<WI>> GetListAsync()
        {
            return await _repository.Entities
                .Include(a => a.Department)
                .Include(a => a.Company)
                .ToListAsync();
        }

        public async Task<int> InsertAsync(WI wi)
        {
            await _repository.AddAsync(wi);
            await _distributedCache.RemoveAsync(CacheKeys.WICacheKeys.ListKey);
            return wi.Id;
        }

        public async Task UpdateAsync(WI wi)
        {
            await _repository.UpdateAsync(wi);
            await _distributedCache.RemoveAsync(CacheKeys.WICacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.WICacheKeys.GetKey(wi.Id));
        }
    }
}