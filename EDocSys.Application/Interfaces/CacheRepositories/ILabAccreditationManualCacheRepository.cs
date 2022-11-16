using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface ILabAccreditationManualCacheRepository
    {
        Task<List<LabAccreditationManual>> GetCachedListAsync();

        Task<LabAccreditationManual> GetByIdAsync(int departmentId);
        Task<LabAccreditationManual> GetByDOCNoAsync(string docno);
    }
}
