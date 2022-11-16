using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface ISafetyHealthManualStatusRepository
    {
        IQueryable<SafetyHealthManualStatus> SafetyHealthManualStatuses { get; }

        Task<List<SafetyHealthManualStatus>> GetListAsync();

        Task<SafetyHealthManualStatus> GetByIdAsync(int safetyHealthManualStatusId);

        Task<int> InsertAsync(SafetyHealthManualStatus safetyHealthManualStatusId);

        Task UpdateAsync(SafetyHealthManualStatus safetyHealthManualStatusId);

        Task DeleteAsync(SafetyHealthManualStatus safetyHealthManualStatusId);

    }
}