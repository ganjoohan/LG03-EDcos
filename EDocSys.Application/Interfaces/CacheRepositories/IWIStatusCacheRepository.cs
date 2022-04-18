using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IWIStatusCacheRepository
    {
        Task<List<WIStatus>> GetCachedListAsync();

        Task<WIStatus> GetByIdAsync(int wiId);
    }
}
