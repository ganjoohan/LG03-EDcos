using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IIssuanceStatusRepository
    {
        IQueryable<IssuanceStatus> IssuanceStatuses { get; }

        Task<List<IssuanceStatus>> GetListAsync();

        Task<IssuanceStatus> GetByIdAsync(int docId);

        Task<int> InsertAsync(IssuanceStatus issuanceStatus);

        Task UpdateAsync(IssuanceStatus issuanceStatus);

        Task DeleteAsync(IssuanceStatus issuanceStatus);

    }
}