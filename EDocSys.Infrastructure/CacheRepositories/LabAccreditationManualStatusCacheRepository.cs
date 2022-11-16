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
    public class LabAccreditationManualStatusCacheRepository : ILabAccreditationManualStatusCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILabAccreditationManualStatusRepository _labAccreditationManualstatusRepository;

        public LabAccreditationManualStatusCacheRepository(IDistributedCache distributedCache, ILabAccreditationManualStatusRepository labAccreditationManualstatusRepository)
        {
            _distributedCache = distributedCache;
            _labAccreditationManualstatusRepository = labAccreditationManualstatusRepository;
        }

        public async Task<LabAccreditationManualStatus> GetByIdAsync(int labAccreditationManualId)
        {
            string cacheKey = LabAccreditationManualStatusCacheKeys.GetKey(labAccreditationManualId);
            var labAccreditationManualstatus = await _distributedCache.GetAsync<LabAccreditationManualStatus>(cacheKey);
            if (labAccreditationManualstatus == null)
            {
                labAccreditationManualstatus = await _labAccreditationManualstatusRepository.GetByIdAsync(labAccreditationManualId);
                Throw.Exception.IfNull(labAccreditationManualstatus, "Lab Accreditation Manual Status", "No Lab Accreditation Manual Status Found");
                await _distributedCache.SetAsync(cacheKey, labAccreditationManualstatus);
            }
            return labAccreditationManualstatus;
        }

        public async Task<List<LabAccreditationManualStatus>> GetCachedListAsync()
        {
            string cacheKey = LabAccreditationManualStatusCacheKeys.ListKey;
            var labAccreditationManualstatusList = await _distributedCache.GetAsync<List<LabAccreditationManualStatus>>(cacheKey);
            if (labAccreditationManualstatusList == null)
            {
                labAccreditationManualstatusList = await _labAccreditationManualstatusRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, labAccreditationManualstatusList);
            }
            return labAccreditationManualstatusList;
        }
    }


}
