using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class DocumentManualRepository : IDocumentManualRepository
    {
        private readonly IRepositoryAsync<DocumentManual> _repository;
        private readonly IDistributedCache _distributedCache;

        public DocumentManualRepository(IDistributedCache distributedCache, IRepositoryAsync<DocumentManual> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<DocumentManual> DocumentManuals => _repository.Entities;

        public async Task DeleteAsync(DocumentManual documentManual)
        {
            await _repository.DeleteAsync(documentManual);
            await _distributedCache.RemoveAsync(CacheKeys.DocumentManualCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.DocumentManualCacheKeys.GetKey(documentManual.Id));
        }

        public async Task<DocumentManual> GetByIdAsync(int documentManualId)
        {
            return await _repository.Entities.Where(p => p.Id == documentManualId)
                .Include(a => a.Company)
                .FirstOrDefaultAsync();
        }

        public async Task<DocumentManual> GetByDOCNoAsync(string docno)
        {
            return await _repository.Entities.Where(p => p.DOCNo == docno)
                .FirstOrDefaultAsync();
        }


        public async Task<List<DocumentManual>> GetListAsync()
        {
            return _repository.Entities
                 .Include(a => a.Company)
                 .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(DocumentManual documentManual)
        {
            await _repository.AddAsync(documentManual);
            await _distributedCache.RemoveAsync(CacheKeys.DocumentManualCacheKeys.ListKey);
            return documentManual.Id;
        }

        public async Task UpdateAsync(DocumentManual documentManual)
        {
            await _repository.UpdateAsync(documentManual);
            await _distributedCache.RemoveAsync(CacheKeys.DocumentManualCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.DocumentManualCacheKeys.GetKey(documentManual.Id));
        }
    }
}