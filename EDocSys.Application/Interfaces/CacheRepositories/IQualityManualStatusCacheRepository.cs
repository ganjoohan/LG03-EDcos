using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IQualityManualStatusCacheRepository
    {
        Task<List<QualityManualStatus>> GetCachedListAsync();

        Task<QualityManualStatus> GetByIdAsync(int docId);
    }
}
