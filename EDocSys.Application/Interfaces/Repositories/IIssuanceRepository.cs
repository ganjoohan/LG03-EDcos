using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IIssuanceRepository
    {
        IQueryable<Issuance> Issuances { get; }

        Task<List<Issuance>> GetListAsync();

        Task<Issuance> GetByIdAsync(int issuanceId);
        Task<Issuance> GetByDOCNoAsync(string docno);

        Task<int> InsertAsync(Issuance issuance);

        Task UpdateAsync(Issuance issuance);

        Task DeleteAsync(Issuance issuance);
    }
}