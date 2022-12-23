using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class IssuanceStatusRepository : IIssuanceStatusRepository
    {
        private readonly IRepositoryAsync<IssuanceStatus> _repository;
        private readonly IDistributedCache _distributedCache;

        public IssuanceStatusRepository(IDistributedCache distributedCache, IRepositoryAsync<IssuanceStatus> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<IssuanceStatus> IssuanceStatuses => _repository.Entities;

        public Task DeleteAsync(IssuanceStatus issuanceStatus)
        {
            throw new System.NotImplementedException();
        }

        //public async Task DeleteAsync(Issuance issuance)
        //{
        //    await _repository.DeleteAsync(issuance);
        //    await _distributedCache.RemoveAsync(CacheKeys.IssuanceCacheKeys.ListKey);
        //    await _distributedCache.RemoveAsync(CacheKeys.IssuanceCacheKeys.GetKey(issuance.Id));
        //}

        public async Task<IssuanceStatus> GetByIdAsync(int docId)
        {
            return await _repository.Entities.Where(p => p.Issuance.Id == docId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<IssuanceStatus>> GetListAsync()
        {
            return await _repository.Entities
                //.Include(a => a.Issuance)
                .Include(a => a.DocumentStatus)
                .ToListAsync();
        }

        public Task<List<IssuanceStatus>> GetListByIdAsync(int docId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> InsertAsync(IssuanceStatus issuanceStatus)
        {
            await _repository.AddAsync(issuanceStatus);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceStatusCacheKeys.ListKey);
            return issuanceStatus.Id;
        }

        public Task UpdateAsync(IssuanceStatus docId)
        {
            throw new System.NotImplementedException();
        }
    }
}