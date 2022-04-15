using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Domain.Entities.UserMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class UserApproverRepository : IUserApproverRepository
    {
        private readonly IRepositoryAsync<UserApprover> _repository;
        private readonly IDistributedCache _distributedCache;

        public UserApproverRepository(IDistributedCache distributedCache, IRepositoryAsync<UserApprover> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<UserApprover> UserApprovers => _repository.Entities;

        public async Task DeleteAsync(UserApprover userapprover)
        {
            await _repository.DeleteAsync(userapprover);
            await _distributedCache.RemoveAsync(CacheKeys.UserApproverCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.UserApproverCacheKeys.GetKey(userapprover.Id));
        }

        public async Task<UserApprover> GetByIdAsync(int userapproverId)
        {
            return await _repository.Entities.Where(p => p.Id == userapproverId).FirstOrDefaultAsync();
        }

        public async Task<List<UserApprover>> GetListAsync()
        {
            // return await _repository.Entities.ToListAsync();

            return await _repository.Entities
               .Include(a => a.Company)
               .Include(a => a.Department)
               .ToListAsync();

        }

        public async Task<int> InsertAsync(UserApprover userapprover)
        {
            await _repository.AddAsync(userapprover);
            await _distributedCache.RemoveAsync(CacheKeys.UserApproverCacheKeys.ListKey);
            return userapprover.Id;
        }

        public async Task UpdateAsync(UserApprover userapprover)
        {
            await _repository.UpdateAsync(userapprover);
            await _distributedCache.RemoveAsync(CacheKeys.UserApproverCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.UserApproverCacheKeys.GetKey(userapprover.Id));
        }
    }
}
