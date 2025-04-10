﻿using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IDocumentManualStatusCacheRepository
    {
        Task<List<DocumentManualStatus>> GetCachedListAsync();

        Task<DocumentManualStatus> GetByIdAsync(int docId);
    }
}
