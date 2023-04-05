using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IProcedureCacheRepository
    {
        Task<List<Procedure>> GetCachedListAsync();

        Task<Procedure> GetByIdAsync(int departmentId);
        Task<Procedure> GetByWSCPNoAsync(string wscpno);
        Task<List<Procedure>> GetByParameterAsync(int companyId,int departmentId);
    }
}
