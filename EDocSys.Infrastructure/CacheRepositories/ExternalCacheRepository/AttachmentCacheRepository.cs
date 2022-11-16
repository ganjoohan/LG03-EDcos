using EDocSys.Application.Interfaces.CacheRepositories;
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Infrastructure.CacheKeys;
using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDocSys.Application.Interfaces.Repositories.ExternalRepositories;
using EDocSys.Domain.Entities.ExternalRecord;
using EDocSys.Application.Interfaces.CacheRepositories.ExternalCacheRepositories;
using EDocSys.Infrastructure.CacheKeys.ExternalCacheKeys;

namespace EDocSys.Infrastructure.CacheRepositories.ExternalCacheRepositories
{
    public class AttachmentCacheRepository : IAttachmentCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IAttachmentRepository _attachmentRepository;

        public AttachmentCacheRepository(IDistributedCache distributedCache, IAttachmentRepository attachmentRepository)
        {
            _distributedCache = distributedCache;
            _attachmentRepository = attachmentRepository;
        }

        public async Task<Attachment> GetByIdAsync(int attachmentId)
        {
            string cacheKey = AttachmentCacheKeys.GetKey(attachmentId);
            var attachment = await _distributedCache.GetAsync<Attachment>(cacheKey);
            if (attachment == null)
            {
                attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
                Throw.Exception.IfNull(attachment, "Attachment", "No Attachment Found");
                await _distributedCache.SetAsync(cacheKey, attachment);
            }
            return attachment;
        }

        public async Task<List<Attachment>> GetCachedListAsync()
        {
            string cacheKey = AttachmentCacheKeys.ListKey;
            var attachmentList = await _distributedCache.GetAsync<List<Attachment>>(cacheKey);
            if (attachmentList == null)
            {
                attachmentList = await _attachmentRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, attachmentList);
            }
            return attachmentList;
        }

        //public async Task<Attachment> GetByDOCNoAsync(string docno)
        //{
        //    string cacheKey = AttachmentCacheKeys.GetKeyDOCNo(docno);
        //    var attachment = await _distributedCache.GetAsync<Attachment>(cacheKey);
        //    if (attachment == null)
        //    {
        //        attachment = await _attachmentRepository.GetByDOCNoAsync(docno);
        //        Throw.Exception.IfNull(attachment, "Attachment", "No Attachment Found");
        //        await _distributedCache.SetAsync(cacheKey, attachment);
        //    }
        //    return attachment;
        //}

        public async Task<List<Attachment>> GetByDocIdAsync(int docid)
        {
            string cacheKey = AttachmentCacheKeys.GetKeyDOCID(docid);
            await _distributedCache.RemoveAsync(CacheKeys.ExternalCacheKeys.AttachmentCacheKeys.GetKeyDOCID(docid));
            var attachment = await _distributedCache.GetAsync<List<Attachment>>(cacheKey);
            if (attachment == null)
            {
                attachment = await _attachmentRepository.GetByDocIdAsync(docid);
                Throw.Exception.IfNull(attachment, "Attachment", "No Attachment Found");
                await _distributedCache.SetAsync(cacheKey, attachment);
            }
            return attachment;
        }

    }
}