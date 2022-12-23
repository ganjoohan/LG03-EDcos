using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class IssuanceRepository : IIssuanceRepository
    {
        private readonly IRepositoryAsync<Issuance> _repository;
        private readonly IDistributedCache _distributedCache;

        public IssuanceRepository(IDistributedCache distributedCache, IRepositoryAsync<Issuance> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<Issuance> Issuances => _repository.Entities;

        public async Task DeleteAsync(Issuance issuance)
        {
            await _repository.DeleteAsync(issuance);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceCacheKeys.GetKey(issuance.Id));
        }

        public async Task<Issuance> GetByIdAsync(int docId)
        {
            return await _repository.Entities.Where(p => p.Id == docId)
                .Include(a => a.Company)
                .FirstOrDefaultAsync();
        }

        public async Task<Issuance> GetByDOCNoAsync(string docno)
        {
            return await _repository.Entities.Where(p => p.DOCNo == docno)
                .FirstOrDefaultAsync();
        }


        public async Task<List<Issuance>> GetListAsync()
        {
            return _repository.Entities
                 .Include(a => a.Company)
                 .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(Issuance issuance)
        {
            await _repository.AddAsync(issuance);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceCacheKeys.ListKey);
            return issuance.Id;
        }

        public async Task UpdateAsync(Issuance issuance)
        {
            await _repository.UpdateAsync(issuance);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceCacheKeys.GetKey(issuance.Id));
        }
    }
}