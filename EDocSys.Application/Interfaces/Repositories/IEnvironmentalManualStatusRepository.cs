using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IEnvironmentalManualStatusRepository
    {
        IQueryable<EnvironmentalManualStatus> EnvironmentalManualStatuses { get; }

        Task<List<EnvironmentalManualStatus>> GetListAsync();

        Task<EnvironmentalManualStatus> GetByIdAsync(int EnvironmentalManualStatusId);

        Task<int> InsertAsync(EnvironmentalManualStatus EnvironmentalManualStatusId);

        Task UpdateAsync(EnvironmentalManualStatus EnvironmentalManualStatusId);

        Task DeleteAsync(EnvironmentalManualStatus EnvironmentalManualStatusId);

    }
}