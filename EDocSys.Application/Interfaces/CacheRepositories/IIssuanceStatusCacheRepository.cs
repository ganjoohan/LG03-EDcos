using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IIssuanceStatusCacheRepository
    {
        Task<List<IssuanceStatus>> GetCachedListAsync();

        Task<IssuanceStatus> GetByIdAsync(int docId);
    }
}
