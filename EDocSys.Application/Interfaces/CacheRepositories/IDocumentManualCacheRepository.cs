using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IDocumentManualCacheRepository
    {
        Task<List<DocumentManual>> GetCachedListAsync();

        Task<DocumentManual> GetByIdAsync(int departmentId);
        Task<DocumentManual> GetByDOCNoAsync(string docno);
    }
}
