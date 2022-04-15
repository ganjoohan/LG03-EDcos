using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IWIRepository
    {
        IQueryable<WI> WIs { get; }

        Task<List<WI>> GetListAsync();

        Task<WI> GetByIdAsync(int wiId);

        Task<int> InsertAsync(WI wi);

        Task UpdateAsync(WI wi);

        Task DeleteAsync(WI wi);
    }
}