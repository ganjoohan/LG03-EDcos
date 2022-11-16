using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IEnvironmentalManualCacheRepository
    {
        Task<List<EnvironmentalManual>> GetCachedListAsync();

        Task<EnvironmentalManual> GetByIdAsync(int departmentId);
        Task<EnvironmentalManual> GetByDOCNoAsync(string docno);
    }
}
