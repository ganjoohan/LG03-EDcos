using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDocSys.Domain.Entities.UserMaster;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IUserApproverRepository
    {
        IQueryable<UserApprover> UserApprovers { get; }

        Task<List<UserApprover>> GetListAsync();

        Task<UserApprover> GetByIdAsync(int userapproverId);

        Task<int> InsertAsync(UserApprover userapprover);

        Task UpdateAsync(UserApprover userapprover);

        Task DeleteAsync(UserApprover userapprover);
    }
}
