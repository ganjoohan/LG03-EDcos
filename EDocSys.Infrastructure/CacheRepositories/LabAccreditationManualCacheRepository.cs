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
    public class LabAccreditationManualCacheRepository : ILabAccreditationManualCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILabAccreditationManualRepository _labAccreditationManualRepository;

        public LabAccreditationManualCacheRepository(IDistributedCache distributedCache, ILabAccreditationManualRepository labAccreditationManualRepository)
        {
            _distributedCache = distributedCache;
            _labAccreditationManualRepository = labAccreditationManualRepository;
        }

        public async Task<LabAccreditationManual> GetByIdAsync(int labAccreditationManualId)
        {
            string cacheKey = LabAccreditationManualCacheKeys.GetKey(labAccreditationManualId);
            var labAccreditationManual = await _distributedCache.GetAsync<LabAccreditationManual>(cacheKey);
            if (labAccreditationManual == null)
            {
                labAccreditationManual = await _labAccreditationManualRepository.GetByIdAsync(labAccreditationManualId);
                Throw.Exception.IfNull(labAccreditationManual, "Lab Accreditation Manual", "No Lab Accreditation Manual Found");
                await _distributedCache.SetAsync(cacheKey, labAccreditationManual);
            }
            return labAccreditationManual;
        }

        public async Task<List<LabAccreditationManual>> GetCachedListAsync()
        {
            string cacheKey = LabAccreditationManualCacheKeys.ListKey;
            var labAccreditationManualList = await _distributedCache.GetAsync<List<LabAccreditationManual>>(cacheKey);
            if (labAccreditationManualList == null)
            {
                labAccreditationManualList = await _labAccreditationManualRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, labAccreditationManualList);
            }
            return labAccreditationManualList;
        }

        public async Task<LabAccreditationManual> GetByDOCNoAsync(string docno)
        {
            string cacheKey = LabAccreditationManualCacheKeys.GetKeyDOCNo(docno);
            var labAccreditationManual = await _distributedCache.GetAsync<LabAccreditationManual>(cacheKey);
            if (labAccreditationManual == null)
            {
                labAccreditationManual = await _labAccreditationManualRepository.GetByDOCNoAsync(docno);
                Throw.Exception.IfNull(labAccreditationManual, "Lab Accreditation Manual", "No Lab Accreditation Manual Found");
                await _distributedCache.SetAsync(cacheKey, labAccreditationManual);
            }
            return labAccreditationManual;
        }

    }
}