using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IIssuanceInfoPrintRepository
    {
        IQueryable<IssuanceInfoPrint> IssuancesInfoPrint { get; }

        Task<List<IssuanceInfoPrint>> GetListAsync();

        Task<IssuanceInfoPrint> GetByIdAsync(int issuanceInfoPrintId);
        Task<List<IssuanceInfoPrint>> GetByHIdAsync(int HId);
        Task<IssuanceInfoPrint> GetByDOCNoAsync(string docno);

        Task<int> InsertAsync(IssuanceInfoPrint issuanceInfoPrint);

        Task UpdateAsync(IssuanceInfoPrint issuanceInfoPrint);

        Task DeleteAsync(IssuanceInfoPrint issuanceInfoPrint);
    }
}