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
    public class DocumentManualStatusCacheRepository : IDocumentManualStatusCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IDocumentManualStatusRepository _documentmanualstatusRepository;

        public DocumentManualStatusCacheRepository(IDistributedCache distributedCache, IDocumentManualStatusRepository documentmanualstatusRepository)
        {
            _distributedCache = distributedCache;
            _documentmanualstatusRepository = documentmanualstatusRepository;
        }

        public async Task<DocumentManualStatus> GetByIdAsync(int documentManualId)
        {
            string cacheKey = DocumentManualStatusCacheKeys.GetKey(documentManualId);
            var documentmanualstatus = await _distributedCache.GetAsync<DocumentManualStatus>(cacheKey);
            if (documentmanualstatus == null)
            {
                documentmanualstatus = await _documentmanualstatusRepository.GetByIdAsync(documentManualId);
                Throw.Exception.IfNull(documentmanualstatus, "Document Manual Status", "No Document Manual Status Found");
                await _distributedCache.SetAsync(cacheKey, documentmanualstatus);
            }
            return documentmanualstatus;
        }

        public async Task<List<DocumentManualStatus>> GetCachedListAsync()
        {
            string cacheKey = DocumentManualStatusCacheKeys.ListKey;
            var documentmanualstatusList = await _distributedCache.GetAsync<List<DocumentManualStatus>>(cacheKey);
            if (documentmanualstatusList == null)
            {
                documentmanualstatusList = await _documentmanualstatusRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, documentmanualstatusList);
            }
            return documentmanualstatusList;
        }
    }


}
