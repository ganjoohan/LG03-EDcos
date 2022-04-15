using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface ISOPStatusRepository
    {
        IQueryable<SOPStatus> SOPStatus { get; }

        Task<List<SOPStatus>> GetListAsync();

        Task<SOPStatus> GetByIdAsync(int sopstatusId);

        Task<int> InsertAsync(SOPStatus sopstatusId);

        Task UpdateAsync(SOPStatus sopstatusId);

        Task DeleteAsync(SOPStatus sopstatusId);

    }
}