using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class IssuanceInfoRepository : IIssuanceInfoRepository
    {
        private readonly IRepositoryAsync<IssuanceInfo> _repository;
        private readonly IDistributedCache _distributedCache;

        public IssuanceInfoRepository(IDistributedCache distributedCache, IRepositoryAsync<IssuanceInfo> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<IssuanceInfo> IssuancesInfo => _repository.Entities;

        public async Task DeleteAsync(IssuanceInfo issuanceInfo)
        {
            await _repository.DeleteAsync(issuanceInfo);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceInfoCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceInfoCacheKeys.GetKey(issuanceInfo.Id));
        }

        public async Task<IssuanceInfo> GetByIdAsync(int docId)
        {
            return await _repository.Entities.Where(p => p.Id == docId)
                //.Include(a => a.Company)
                .FirstOrDefaultAsync();
        }
        public async Task<List<IssuanceInfo>> GetByHIdAsync(int HId)
        {
            return  _repository.Entities.Where(p => p.HId == HId)
                .ToList();
        }
        public async Task<IssuanceInfo> GetByDOCNoAsync(string docno)
        {
            return await _repository.Entities.Where(p => p.DOCNo == docno)
                .FirstOrDefaultAsync();
        }


        public async Task<List<IssuanceInfo>> GetListAsync()
        {
            return _repository.Entities
                 //.Include(a => a.Company)
                 .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(IssuanceInfo issuanceInfo)
        {
            await _repository.AddAsync(issuanceInfo);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceInfoCacheKeys.ListKey);
            return issuanceInfo.Id;
        }

        public async Task UpdateAsync(IssuanceInfo issuanceInfo)
        {
            await _repository.UpdateAsync(issuanceInfo);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceInfoCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceInfoCacheKeys.GetKey(issuanceInfo.Id));
        }
    }
}