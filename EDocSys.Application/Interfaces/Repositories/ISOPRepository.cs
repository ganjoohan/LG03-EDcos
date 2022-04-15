using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface ISOPRepository
    {
        IQueryable<SOP> SOPs { get; }

        Task<List<SOP>> GetListAsync();

        Task<SOP> GetByIdAsync(int sopId);

        Task<int> InsertAsync(SOP sop);

        Task UpdateAsync(SOP sop);

        Task DeleteAsync(SOP sop);
    }
}