using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class DocumentManualStatusRepository : IDocumentManualStatusRepository
    {
        private readonly IRepositoryAsync<DocumentManualStatus> _repository;
        private readonly IDistributedCache _distributedCache;

        public DocumentManualStatusRepository(IDistributedCache distributedCache, IRepositoryAsync<DocumentManualStatus> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<DocumentManualStatus> DocumentManualStatuses => _repository.Entities;

        public Task DeleteAsync(DocumentManualStatus documentmanualstatusId)
        {
            throw new System.NotImplementedException();
        }

        //public async Task DeleteAsync(DocumentManual documentManual)
        //{
        //    await _repository.DeleteAsync(documentManual);
        //    await _distributedCache.RemoveAsync(CacheKeys.DocumentManualCacheKeys.ListKey);
        //    await _distributedCache.RemoveAsync(CacheKeys.DocumentManualCacheKeys.GetKey(documentManual.Id));
        //}

        public async Task<DocumentManualStatus> GetByIdAsync(int documentManualId)
        {
            return await _repository.Entities.Where(p => p.DocumentManual.Id == documentManualId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DocumentManualStatus>> GetListAsync()
        {
            return await _repository.Entities
                //.Include(a => a.DocumentManual)
                .Include(a => a.DocumentStatus)
                .ToListAsync();
        }

        public Task<List<DocumentManualStatus>> GetListByIdAsync(int documentManualId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> InsertAsync(DocumentManualStatus documentmanualstatusId)
        {
            await _repository.AddAsync(documentmanualstatusId);
            await _distributedCache.RemoveAsync(CacheKeys.DocumentManualStatusCacheKeys.ListKey);
            return documentmanualstatusId.Id;
        }

        public Task UpdateAsync(DocumentManualStatus documentmanualstatusId)
        {
            throw new System.NotImplementedException();
        }
    }
}