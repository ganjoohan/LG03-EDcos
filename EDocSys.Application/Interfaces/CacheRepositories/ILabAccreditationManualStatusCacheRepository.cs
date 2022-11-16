using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface ILabAccreditationManualStatusCacheRepository
    {
        Task<List<LabAccreditationManualStatus>> GetCachedListAsync();

        Task<LabAccreditationManualStatus> GetByIdAsync(int docId);
    }
}
