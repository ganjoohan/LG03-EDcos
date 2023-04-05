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
    public class IssuanceInfoCacheRepository : IIssuanceInfoCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IIssuanceInfoRepository _issuanceInfoRepository;

        public IssuanceInfoCacheRepository(IDistributedCache distributedCache, IIssuanceInfoRepository issuanceInfoRepository)
        {
            _distributedCache = distributedCache;
            _issuanceInfoRepository = issuanceInfoRepository;
        }

        public async Task<IssuanceInfo> GetByIdAsync(int docId)
        {
            string cacheKey = IssuanceInfoCacheKeys.GetKey(docId);
            var issuanceInfo = await _distributedCache.GetAsync<IssuanceInfo>(cacheKey);
            if (issuanceInfo == null)
            {
                issuanceInfo = await _issuanceInfoRepository.GetByIdAsync(docId);
                Throw.Exception.IfNull(issuanceInfo, "IssuanceInfo", "No IssuanceInfo Found");
                await _distributedCache.SetAsync(cacheKey, issuanceInfo);
            }
            return issuanceInfo;
        }

        public async Task<List<IssuanceInfo>> GetByHIdAsync(int HId)
        {
            var issuanceInfo = await _issuanceInfoRepository.GetByHIdAsync(HId);
            return issuanceInfo;
        }

        public async Task<List<IssuanceInfo>> GetCachedListAsync()
        {
            string cacheKey = IssuanceInfoCacheKeys.ListKey;
            var issuanceInfoList = await _distributedCache.GetAsync<List<IssuanceInfo>>(cacheKey);
            if (issuanceInfoList == null)
            {
                issuanceInfoList = await _issuanceInfoRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, issuanceInfoList);
            }
            return issuanceInfoList;
        }

        public async Task<List<IssuanceInfo>> GetByDOCNoAsync(string docNo, string docType)
        {
            string cacheKey = IssuanceInfoCacheKeys.GetKeyDOCNo(docNo);
            var issuanceInfo = await _distributedCache.GetAsync<List<IssuanceInfo>>(cacheKey);
            if (issuanceInfo == null)
            {
                issuanceInfo = await _issuanceInfoRepository.GetByDOCNoAsync(docNo, docType);
                Throw.Exception.IfNull(issuanceInfo, "IssuanceInfo", "No IssuanceInfo Found");
                await _distributedCache.SetAsync(cacheKey, issuanceInfo);
            }
            return issuanceInfo;
        }

    }
}