using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IProcedureRepository
    {
        IQueryable<Procedure> Procedures { get; }

        Task<List<Procedure>> GetListAsync();

        Task<Procedure> GetByIdAsync(int procedureId);
        Task<Procedure> GetByWSCPNoAsync(string wscpno);

        Task<int> InsertAsync(Procedure procedure);

        Task UpdateAsync(Procedure procedure);

        Task DeleteAsync(Procedure procedure);
    }
}