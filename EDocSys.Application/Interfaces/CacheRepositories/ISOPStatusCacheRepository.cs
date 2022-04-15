using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface ISOPStatusCacheRepository
    {
        Task<List<SOPStatus>> GetCachedListAsync();

        Task<SOPStatus> GetByIdAsync(int sopId);
    }
}
