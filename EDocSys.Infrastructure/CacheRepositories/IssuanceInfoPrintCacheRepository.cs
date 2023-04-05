using EDocSys.Application.Interfaces.CacheRepositories;
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Infrastructure.CacheKeys;
using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.CacheRepositories
{
    public class IssuanceInfoPrintCacheRepository : IIssuanceInfoPrintCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IIssuanceInfoPrintRepository _issuanceInfoPrintRepository;

        public IssuanceInfoPrintCacheRepository(IDistributedCache distributedCache, IIssuanceInfoPrintRepository issuanceInfoPrintRepository)
        {
            _distributedCache = distributedCache;
            _issuanceInfoPrintRepository = issuanceInfoPrintRepository;
        }

        public async Task<IssuanceInfoPrint> GetByIdAsync(int docId)
        {
            string cacheKey = IssuanceInfoPrintCacheKeys.GetKey(docId);
            var issuanceInfoPrint = await _distributedCache.GetAsync<IssuanceInfoPrint>(cacheKey);
            if (issuanceInfoPrint == null)
            {
                issuanceInfoPrint = await _issuanceInfoPrintRepository.GetByIdAsync(docId);
                Throw.Exception.IfNull(issuanceInfoPrint, "IssuanceInfoPrint", "No IssuanceInfoPrint Found");
                await _distributedCache.SetAsync(cacheKey, issuanceInfoPrint);
            }
            return issuanceInfoPrint;
        }

        public async Task<List<IssuanceInfoPrint>> GetByHIdAsync(int HId)
        {
            var issuanceInfoPrint = await _issuanceInfoPrintRepository.GetByHIdAsync(HId);
            return issuanceInfoPrint;
        }

        public async Task<List<IssuanceInfoPrint>> GetCachedListAsync()
        {
            string cacheKey = IssuanceInfoPrintCacheKeys.ListKey;
            var issuanceInfoPrintList = await _distributedCache.GetAsync<List<IssuanceInfoPrint>>(cacheKey);
            if (issuanceInfoPrintList == null)
            {
                issuanceInfoPrintList = await _issuanceInfoPrintRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, issuanceInfoPrintList);
            }
            return issuanceInfoPrintList;
        }

        public async Task<IssuanceInfoPrint> GetByDOCNoAsync(string docno)
        {
            string cacheKey = IssuanceInfoPrintCacheKeys.GetKeyDOCNo(docno);
            var issuanceInfoPrint = await _distributedCache.GetAsync<IssuanceInfoPrint>(cacheKey);
            if (issuanceInfoPrint == null)
            {
                issuanceInfoPrint = await _issuanceInfoPrintRepository.GetByDOCNoAsync(docno);
                Throw.Exception.IfNull(issuanceInfoPrint, "IssuanceInfoPrint", "No IssuanceInfoPrint Found");
                await _distributedCache.SetAsync(cacheKey, issuanceInfoPrint);
            }
            return issuanceInfoPrint;
        }

    }
}