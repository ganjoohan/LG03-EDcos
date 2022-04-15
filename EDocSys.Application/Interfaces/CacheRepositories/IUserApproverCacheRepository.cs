using System.Collections.Generic;
using System.Threading.Tasks;
using EDocSys.Domain.Entities.UserMaster;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IUserApproverCacheRepository
    {
        Task<List<UserApprover>> GetCachedListAsync();

        Task<UserApprover> GetByIdAsync(int userapproverId);
    }
}
