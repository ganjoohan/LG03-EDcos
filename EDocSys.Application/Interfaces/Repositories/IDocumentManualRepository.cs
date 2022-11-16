using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IDocumentManualRepository
    {
        IQueryable<DocumentManual> DocumentManuals { get; }

        Task<List<DocumentManual>> GetListAsync();

        Task<DocumentManual> GetByIdAsync(int documentManualId);
        Task<DocumentManual> GetByDOCNoAsync(string docno);

        Task<int> InsertAsync(DocumentManual documentManual);

        Task UpdateAsync(DocumentManual documentManual);

        Task DeleteAsync(DocumentManual documentManual);
    }
}