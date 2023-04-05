using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IWICacheRepository
    {
        Task<List<WI>> GetCachedListAsync();

        Task<WI> GetByIdAsync(int departmentId);

        Task<List<WI>> GetByParameterAsync(int companyId, int departmentId);
    }
}
