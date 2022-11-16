using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Application.Interfaces.Repositories.QualityRepositories;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Domain.Entities.QualityRecord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories.QualityRepositories
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

        public IQueryable<Attachment> Attachments => _repository.EntitiesQuality;

        public async Task DeleteAsync(Attachment attachment)
        {
            await _repository.DeleteQualityAsync(attachment);
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.AttachmentCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.AttachmentCacheKeys.GetKey(attachment.Id));
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.AttachmentCacheKeys.GetKeyDOCID(attachment.DocId));
        }

        public async Task<Attachment> GetByIdAsync(int attachmentId)
        {
            return await _repository.EntitiesQuality.Where(p => p.Id == attachmentId)
                //.Include(a => a.Company)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Attachment>> GetByDocIdAsync(int docId)
        {
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.AttachmentCacheKeys.GetKeyDOCID(docId));
            return await _repository.EntitiesQuality.Where(p => p.DocId == docId)
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
            return _repository.EntitiesQuality
               //.Include(a => a.Company)
               .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(Attachment attachment)
        {
            await _repository.AddQualityAsync(attachment);
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.AttachmentCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.AttachmentCacheKeys.GetKeyDOCID(attachment.DocId));
            return attachment.Id;
        }

        public async Task UpdateAsync(Attachment attachment)
        {
            await _repository.UpdateQualityAsync(attachment);
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.AttachmentCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.AttachmentCacheKeys.GetKey(attachment.Id));
            await _distributedCache.RemoveAsync(CacheKeys.QualityCacheKeys.AttachmentCacheKeys.GetKeyDOCID(attachment.DocId));
        }
    }
}