using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class WIStatusRepository : IWIStatusRepository
    {
        private readonly IRepositoryAsync<WIStatus> _repository;
        private readonly IDistributedCache _distributedCache;

        public WIStatusRepository(IDistributedCache distributedCache, IRepositoryAsync<WIStatus> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<WIStatus> WIStatus => _repository.Entities;

        public Task DeleteAsync(WIStatus wistatusId)
        {
            throw new System.NotImplementedException();
        }

        //public async Task DeleteAsync(WI wi)
        //{
        //    await _repository.DeleteAsync(wi);
        //    await _distributedCache.RemoveAsync(CacheKeys.WICacheKeys.ListKey);
        //    await _distributedCache.RemoveAsync(CacheKeys.WICacheKeys.GetKey(wi.Id));
        //}

        public async Task<WIStatus> GetByIdAsync(int wiId)
        {
            return await _repository.Entities.Where(p => p.WI.Id == wiId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<WIStatus>> GetListAsync()
        {
            return await _repository.Entities
                //.Include(a => a.WI)
                .Include(a => a.DocumentStatus)
                .ToListAsync();
        }

        public Task<List<WIStatus>> GetListByIdAsync(int wiId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> InsertAsync(WIStatus wistatusId)
        {
            await _repository.AddAsync(wistatusId);
            await _distributedCache.RemoveAsync(CacheKeys.WIStatusCacheKeys.ListKey);
            return wistatusId.Id;
        }

        public Task UpdateAsync(WIStatus wistatusId)
        {
            throw new System.NotImplementedException();
        }
    }
}