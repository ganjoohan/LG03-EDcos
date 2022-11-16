using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IQualityManualRepository
    {
        IQueryable<QualityManual> QualityManuals { get; }

        Task<List<QualityManual>> GetListAsync();

        Task<QualityManual> GetByIdAsync(int qualityManualId);
        Task<QualityManual> GetByDOCNoAsync(string docno);

        Task<int> InsertAsync(QualityManual qualityManual);

        Task UpdateAsync(QualityManual qualityManual);

        Task DeleteAsync(QualityManual qualityManual);
    }
}