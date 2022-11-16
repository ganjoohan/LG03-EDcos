using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IQualityManualCacheRepository
    {
        Task<List<QualityManual>> GetCachedListAsync();

        Task<QualityManual> GetByIdAsync(int departmentId);
        Task<QualityManual> GetByDOCNoAsync(string docno);
    }
}
