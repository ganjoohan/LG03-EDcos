using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Application.Interfaces.Repositories.QualityRepositories;
using EDocSys.Domain.Entities.QualityRecord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories.QualityRepositories
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

        public IQueryable<LionSteel> LionSteels => _repository.EntitiesQuality;

        public async Task DeleteAsync(LionSteel lionSteel)
        {
            await _repository.DeleteQualityAsync(lionSteel);
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.LionSteelCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.LionSteelCacheKeys.GetKey(lionSteel.Id));
        }

        public async Task<LionSteel> GetByIdAsync(int lionSteelId)
        {
            return await _repository.EntitiesQuality.Where(p => p.Id == lionSteelId).FirstOrDefaultAsync();
        }
        public async Task<LionSteel> GetByDOCNoAsync(string docno)
        {
            return await _repository.EntitiesQuality.Where(p => p.FormNo == docno)
                 .FirstOrDefaultAsync();
        }

        public async Task<List<LionSteel>> GetListAsync()
        {
            return await _repository.EntitiesQuality.ToListAsync();
        }

        public async Task<int> InsertAsync(LionSteel lionSteel)
        {
            await _repository.AddQualityAsync(lionSteel);
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.LionSteelCacheKeys.ListKey);
            return lionSteel.Id;
        }

        public async Task UpdateAsync(LionSteel lionSteel)
        {
            await _repository.UpdateQualityAsync(lionSteel);
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.LionSteelCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.LionSteelCacheKeys.GetKey(lionSteel.Id));
        }
    }
}
