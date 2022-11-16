using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IEnvironmentalManualStatusCacheRepository
    {
        Task<List<EnvironmentalManualStatus>> GetCachedListAsync();

        Task<EnvironmentalManualStatus> GetByIdAsync(int docId);
    }
}
