using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Application.Interfaces.Repositories.ExternalRepositories;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Domain.Entities.ExternalRecord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories.ExternalRepositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly IRepositoryAsync<Attachment> _repository;
        private readonly IDistributedCache _distributedCache;

        public AttachmentRepository(IDistributedCache distributedCache, IRepositoryAsync<Attachment> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<Attachment> Attachments => _repository.EntitiesExternal;

        public async Task DeleteAsync(Attachment attachment)
        {
            await _repository.DeleteExternalAsync(attachment);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.AttachmentCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.AttachmentCacheKeys.GetKey(attachment.Id));
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.AttachmentCacheKeys.GetKeyDOCID(attachment.DocId));
        }

        public async Task<Attachment> GetByIdAsync(int attachmentId)
        {
            return await _repository.EntitiesExternal.Where(p => p.Id == attachmentId)
                //.Include(a => a.Company)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Attachment>> GetByDocIdAsync(int docId)
        {
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.AttachmentCacheKeys.GetKeyDOCID(docId));
            return await _repository.EntitiesExternal.Where(p => p.DocId == docId)
                //.Include(a => a.Company)
                .ToListAsync();
        }
        //public async Task<Attachment> GetByDOCNoAsync(string docno)
        //{
        //    return await _repository.Entities.Where(p => p.DOCNo == docno)
        //        .FirstOrDefaultAsync();
        //}


        public async Task<List<Attachment>> GetListAsync()
        {
            return _repository.EntitiesExternal
               //.Include(a => a.Company)
               .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(Attachment attachment)
        {
            await _repository.AddExternalAsync(attachment);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.AttachmentCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.AttachmentCacheKeys.GetKeyDOCID(attachment.DocId));
            return attachment.Id;
        }

        public async Task UpdateAsync(Attachment attachment)
        {
            await _repository.UpdateExternalAsync(attachment);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.AttachmentCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.AttachmentCacheKeys.GetKey(attachment.Id));
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.AttachmentCacheKeys.GetKeyDOCID(attachment.DocId));
        }
    }
}