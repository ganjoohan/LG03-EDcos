using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IDocumentManualStatusRepository
    {
        IQueryable<DocumentManualStatus> DocumentManualStatuses { get; }

        Task<List<DocumentManualStatus>> GetListAsync();

        Task<DocumentManualStatus> GetByIdAsync(int documentManualStatusId);

        Task<int> InsertAsync(DocumentManualStatus documentManualStatusId);

        Task UpdateAsync(DocumentManualStatus documentManualStatusId);

        Task DeleteAsync(DocumentManualStatus documentManualStatusId);

    }
}