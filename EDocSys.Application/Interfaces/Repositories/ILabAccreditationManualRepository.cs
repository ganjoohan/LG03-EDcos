using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface ILabAccreditationManualRepository
    {
        IQueryable<LabAccreditationManual> LabAccreditationManuals { get; }

        Task<List<LabAccreditationManual>> GetListAsync();

        Task<LabAccreditationManual> GetByIdAsync(int LabAccreditationManualId);
        Task<LabAccreditationManual> GetByDOCNoAsync(string docno);

        Task<int> InsertAsync(LabAccreditationManual LabAccreditationManual);

        Task UpdateAsync(LabAccreditationManual LabAccreditationManual);

        Task DeleteAsync(LabAccreditationManual LabAccreditationManual);
    }
}