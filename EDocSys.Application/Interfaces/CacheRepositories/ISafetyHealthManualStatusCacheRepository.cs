using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface ISafetyHealthManualStatusCacheRepository
    {
        Task<List<SafetyHealthManualStatus>> GetCachedListAsync();

        Task<SafetyHealthManualStatus> GetByIdAsync(int docId);
    }
}
