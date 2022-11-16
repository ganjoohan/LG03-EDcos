using EDocSys.Domain.Entities.QualityRecord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories.QualityCacheRepositories
{
    public interface ILionSteelCacheRepository
    {
        Task<List<LionSteel>> GetCachedListAsync();

        Task<LionSteel> GetByIdAsync(int lionSteelId);
        Task<LionSteel> GetByDOCNoAsync(string docno);
    }
}
