using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IIssuanceInfoCacheRepository
    {
        Task<List<IssuanceInfo>> GetCachedListAsync();

        Task<IssuanceInfo> GetByIdAsync(int Id);
        Task<List<IssuanceInfo>> GetByHIdAsync(int HId);
        Task<IssuanceInfo> GetByDOCNoAsync(string docno);
    }
}
