using EDocSys.Domain.Entities.ExternalRecord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories.ExternalCacheRepositories
{
    public interface ILionSteelCacheRepository
    {
        Task<List<LionSteel>> GetCachedListAsync();

        Task<LionSteel> GetByIdAsync(int lionSteelId);
        Task<LionSteel> GetByDOCNoAsync(string docno);
    }
}
