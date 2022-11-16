using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class LabAccreditationManualStatusRepository : ILabAccreditationManualStatusRepository
    {
        private readonly IRepositoryAsync<LabAccreditationManualStatus> _repository;
        private readonly IDistributedCache _distributedCache;

        public LabAccreditationManualStatusRepository(IDistributedCache distributedCache, IRepositoryAsync<LabAccreditationManualStatus> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<LabAccreditationManualStatus> LabAccreditationManualStatuses => _repository.Entities;

        public Task DeleteAsync(LabAccreditationManualStatus labAccreditationManualstatusId)
        {
            throw new System.NotImplementedException();
        }

        //public async Task DeleteAsync(LabAccreditationManual LabAccreditationManual)
        //{
        //    await _repository.DeleteAsync(LabAccreditationManual);
        //    await _distributedCache.RemoveAsync(CacheKeys.LabAccreditationManualCacheKeys.ListKey);
        //    await _distributedCache.RemoveAsync(CacheKeys.LabAccreditationManualCacheKeys.GetKey(LabAccreditationManual.Id));
        //}

        public async Task<LabAccreditationManualStatus> GetByIdAsync(int labAccreditationManualId)
        {
            return await _repository.Entities.Where(p => p.LabAccreditationManual.Id == labAccreditationManualId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<LabAccreditationManualStatus>> GetListAsync()
        {
            return await _repository.Entities
                //.Include(a => a.LabAccreditationManual)
                .Include(a => a.DocumentStatus)
                .ToListAsync();
        }

        public Task<List<LabAccreditationManualStatus>> GetListByIdAsync(int labAccreditationManualId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> InsertAsync(LabAccreditationManualStatus labAccreditationManualstatusId)
        {
            await _repository.AddAsync(labAccreditationManualstatusId);
            await _distributedCache.RemoveAsync(CacheKeys.LabAccreditationManualStatusCacheKeys.ListKey);
            return labAccreditationManualstatusId.Id;
        }

        public Task UpdateAsync(LabAccreditationManualStatus labAccreditationManualstatusId)
        {
            throw new System.NotImplementedException();
        }
    }
}