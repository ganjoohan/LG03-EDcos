using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IIssuanceCacheRepository
    {
        Task<List<Issuance>> GetCachedListAsync();

        Task<Issuance> GetByIdAsync(int departmentId);
        Task<Issuance> GetByDOCNoAsync(string docno);
    }
}
