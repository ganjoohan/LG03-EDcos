using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Application.Interfaces.Repositories.ExternalRepositories;
using EDocSys.Domain.Entities.ExternalRecord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories.ExternalRepositories
{
    public class LionSteelRepository : ILionSteelRepository
    {
        private readonly IRepositoryAsync<LionSteel> _repository;
        private readonly IDistributedCache _distributedCache;

        public LionSteelRepository(IDistributedCache distributedCache, IRepositoryAsync<LionSteel> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<LionSteel> LionSteels => _repository.EntitiesExternal;

        public async Task DeleteAsync(LionSteel lionSteel)
        {
            await _repository.DeleteExternalAsync(lionSteel);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.LionSteelCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.LionSteelCacheKeys.GetKey(lionSteel.Id));
        }

        public async Task<LionSteel> GetByDOCNoAsync(string docno)
        {
            return await _repository.EntitiesExternal.Where(p => p.FormNo == docno)
                 .FirstOrDefaultAsync();
        }

        public async Task<LionSteel> GetByIdAsync(int lionSteelId)
        {
            return await _repository.EntitiesExternal.Where(p => p.Id == lionSteelId).FirstOrDefaultAsync();
        }

        public async Task<List<LionSteel>> GetListAsync()
        {
            return await _repository.EntitiesExternal.ToListAsync();
        }

        public async Task<int> InsertAsync(LionSteel lionSteel)
        {
            await _repository.AddExternalAsync(lionSteel);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.LionSteelCacheKeys.ListKey);
            return lionSteel.Id;
        }

        public async Task UpdateAsync(LionSteel lionSteel)
        {
            await _repository.UpdateExternalAsync(lionSteel);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.LionSteelCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.LionSteelCacheKeys.GetKey(lionSteel.Id));
        }
    }
}
