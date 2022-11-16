using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface ISafetyHealthManualRepository
    {
        IQueryable<SafetyHealthManual> SafetyHealthManuals { get; }

        Task<List<SafetyHealthManual>> GetListAsync();

        Task<SafetyHealthManual> GetByIdAsync(int safetyHealthManualId);
        Task<SafetyHealthManual> GetByDOCNoAsync(string docno);

        Task<int> InsertAsync(SafetyHealthManual safetyHealthManual);

        Task UpdateAsync(SafetyHealthManual safetyHealthManual);

        Task DeleteAsync(SafetyHealthManual safetyHealthManual);
    }
}