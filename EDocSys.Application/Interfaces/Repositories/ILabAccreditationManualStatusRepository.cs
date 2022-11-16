using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface ILabAccreditationManualStatusRepository
    {
        IQueryable<LabAccreditationManualStatus> LabAccreditationManualStatuses { get; }

        Task<List<LabAccreditationManualStatus>> GetListAsync();

        Task<LabAccreditationManualStatus> GetByIdAsync(int LabAccreditationManualStatusId);

        Task<int> InsertAsync(LabAccreditationManualStatus LabAccreditationManualStatusId);

        Task UpdateAsync(LabAccreditationManualStatus LabAccreditationManualStatusId);

        Task DeleteAsync(LabAccreditationManualStatus LabAccreditationManualStatusId);

    }
}