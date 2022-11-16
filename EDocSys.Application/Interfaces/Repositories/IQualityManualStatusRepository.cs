using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IQualityManualStatusRepository
    {
        IQueryable<QualityManualStatus> QualityManualStatuses { get; }

        Task<List<QualityManualStatus>> GetListAsync();

        Task<QualityManualStatus> GetByIdAsync(int qualityManualStatusId);

        Task<int> InsertAsync(QualityManualStatus qualityManualStatusId);

        Task UpdateAsync(QualityManualStatus qualityManualStatusId);

        Task DeleteAsync(QualityManualStatus qualityManualStatusId);

    }
}