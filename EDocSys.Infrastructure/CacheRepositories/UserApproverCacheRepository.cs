using EDocSys.Application.Interfaces.CacheRepositories;
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Domain.Entities.UserMaster;
using EDocSys.Infrastructure.CacheKeys;
using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.CacheRepositories
{
    public class UserApproverCacheRepository : IUserApproverCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IUserApproverRepository _userapproverRepository;

        public UserApproverCacheRepository(IDistributedCache distributedCache, IUserApproverRepository userapproverRepository)
        {
            _distributedCache = distributedCache;
            _userapproverRepository = userapproverRepository;
        }

        public async Task<UserApprover> GetByIdAsync(int userapproverId)
        {
            string cacheKey = UserApproverCacheKeys.GetKey(userapproverId);
            var userapprover = await _distributedCache.GetAsync<UserApprover>(cacheKey);
            if (userapprover == null)
            {
                userapprover = await _userapproverRepository.GetByIdAsync(userapproverId);
                Throw.Exception.IfNull(userapprover, "UserApprover", "No UserApprover Found");
                await _distributedCache.SetAsync(cacheKey, userapprover);
            }
            return userapprover;
        }

        public async Task<List<UserApprover>> GetCachedListAsync()
        {
            string cacheKey = UserApproverCacheKeys.ListKey;
            var userapproverList = await _distributedCache.GetAsync<List<UserApprover>>(cacheKey);
            if (userapproverList == null)
            {
                userapproverList = await _userapproverRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, userapproverList);
            }
            return userapproverList;
        }
    }
}
