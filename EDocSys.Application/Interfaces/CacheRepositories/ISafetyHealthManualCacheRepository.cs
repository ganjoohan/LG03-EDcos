using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface ISafetyHealthManualCacheRepository
    {
        Task<List<SafetyHealthManual>> GetCachedListAsync();

        Task<SafetyHealthManual> GetByIdAsync(int departmentId);
        Task<SafetyHealthManual> GetByDOCNoAsync(string docno);
    }
}
