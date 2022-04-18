using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IWIStatusRepository
    {
        IQueryable<WIStatus> WIStatus { get; }

        Task<List<WIStatus>> GetListAsync();

        Task<WIStatus> GetByIdAsync(int wistatusId);

        Task<int> InsertAsync(WIStatus wistatusId);

        Task UpdateAsync(WIStatus wistatusId);

        Task DeleteAsync(WIStatus wistatusId);

    }
}