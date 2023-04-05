using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface ISOPCacheRepository
    {
        Task<List<SOP>> GetCachedListAsync();

        Task<SOP> GetByIdAsync(int departmentId);

        Task<List<SOP>> GetByParameterAsync(int companyId, int departmentId);
    }
}
