using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IProcedureStatusCacheRepository
    {
        Task<List<ProcedureStatus>> GetCachedListAsync();

        Task<ProcedureStatus> GetByIdAsync(int procedureId);
    }
}
