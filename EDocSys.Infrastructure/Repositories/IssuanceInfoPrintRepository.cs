using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class IssuanceInfoPrintRepository : IIssuanceInfoPrintRepository
    {
        private readonly IRepositoryAsync<IssuanceInfoPrint> _repository;
        private readonly IDistributedCache _distributedCache;

        public IssuanceInfoPrintRepository(IDistributedCache distributedCache, IRepositoryAsync<IssuanceInfoPrint> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<IssuanceInfoPrint> IssuancesInfoPrint => _repository.Entities;

        public async Task DeleteAsync(IssuanceInfoPrint issuanceInfoPrint)
        {
            await _repository.DeleteAsync(issuanceInfoPrint);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceInfoPrintCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceInfoPrintCacheKeys.GetKey(issuanceInfoPrint.Id));
        }

        public async Task<IssuanceInfoPrint> GetByIdAsync(int docId)
        {
            return await _repository.Entities.Where(p => p.Id == docId)
                //.Include(a => a.Company)
                .FirstOrDefaultAsync();
        }
        public async Task<List<IssuanceInfoPrint>> GetByHIdAsync(int HId)
        {
            return  _repository.Entities.Where(p => p.IssInfoId == HId)
                .ToList();
        }
        public async Task<IssuanceInfoPrint> GetByDOCNoAsync(string docno) //not in use (Elaine Ho 9Mac2023)
        {
            return await _repository.Entities.Where(p => p.IssInfoId.ToString() == docno)
                .FirstOrDefaultAsync();
        }


        public async Task<List<IssuanceInfoPrint>> GetListAsync()
        {
            return _repository.Entities
                 //.Include(a => a.Company)
                 .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(IssuanceInfoPrint issuanceInfoPrint)
        {
            await _repository.AddAsync(issuanceInfoPrint);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceInfoPrintCacheKeys.ListKey);
            return issuanceInfoPrint.Id;
        }

        public async Task UpdateAsync(IssuanceInfoPrint issuanceInfoPrint)
        {
            await _repository.UpdateAsync(issuanceInfoPrint);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceInfoPrintCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.IssuanceInfoPrintCacheKeys.GetKey(issuanceInfoPrint.Id));
        }
    }
}