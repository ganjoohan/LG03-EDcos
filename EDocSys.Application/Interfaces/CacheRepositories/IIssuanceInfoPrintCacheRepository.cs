using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IIssuanceInfoPrintCacheRepository
    {
        Task<List<IssuanceInfoPrint>> GetCachedListAsync();

        Task<IssuanceInfoPrint> GetByIdAsync(int Id);
        Task<List<IssuanceInfoPrint>> GetByHIdAsync(int HId);
        Task<IssuanceInfoPrint> GetByDOCNoAsync(string docno);
    }
}
