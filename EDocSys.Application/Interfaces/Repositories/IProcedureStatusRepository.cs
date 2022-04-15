using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IProcedureStatusRepository
    {
        IQueryable<ProcedureStatus> ProcedureStatus { get; }

        Task<List<ProcedureStatus>> GetListAsync();

        Task<ProcedureStatus> GetByIdAsync(int procedurestatusId);

        Task<int> InsertAsync(ProcedureStatus procedurestatusId);

        Task UpdateAsync(ProcedureStatus procedurestatusId);

        Task DeleteAsync(ProcedureStatus procedurestatusId);

    }
}