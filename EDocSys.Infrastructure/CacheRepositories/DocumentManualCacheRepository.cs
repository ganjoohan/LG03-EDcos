using EDocSys.Application.Interfaces.CacheRepositories;
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Infrastructure.CacheKeys;
using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.CacheRepositories
{
    public class DocumentManualCacheRepository : IDocumentManualCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IDocumentManualRepository _documentManualRepository;

        public DocumentManualCacheRepository(IDistributedCache distributedCache, IDocumentManualRepository documentManualRepository)
        {
            _distributedCache = distributedCache;
            _documentManualRepository = documentManualRepository;
        }

        public async Task<DocumentManual> GetByIdAsync(int documentManualId)
        {
            string cacheKey = DocumentManualCacheKeys.GetKey(documentManualId);
            var documentManual = await _distributedCache.GetAsync<DocumentManual>(cacheKey);
            if (documentManual == null)
            {
                documentManual = await _documentManualRepository.GetByIdAsync(documentManualId);
                Throw.Exception.IfNull(documentManual, "Document Manual", "No Document Manual Found");
                await _distributedCache.SetAsync(cacheKey, documentManual);
            }
            return documentManual;
        }

        public async Task<List<DocumentManual>> GetCachedListAsync()
        {
            string cacheKey = DocumentManualCacheKeys.ListKey;
            var documentManualList = await _distributedCache.GetAsync<List<DocumentManual>>(cacheKey);
            if (documentManualList == null)
            {
                documentManualList = await _documentManualRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, documentManualList);
            }
            return documentManualList;
        }

        public async Task<DocumentManual> GetByDOCNoAsync(string docno)
        {
            string cacheKey = DocumentManualCacheKeys.GetKeyDOCNo(docno);
            var documentManual = await _distributedCache.GetAsync<DocumentManual>(cacheKey);
            if (documentManual == null)
            {
                documentManual = await _documentManualRepository.GetByDOCNoAsync(docno);
                Throw.Exception.IfNull(documentManual, "Document Manual", "No Document Manual Found");
                await _distributedCache.SetAsync(cacheKey, documentManual);
            }
            return documentManual;
        }

    }
}