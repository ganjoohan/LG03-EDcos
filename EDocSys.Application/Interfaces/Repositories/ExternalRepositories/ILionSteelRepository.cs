using EDocSys.Domain.Entities.ExternalRecord;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories.ExternalRepositories
{
    public interface ILionSteelRepository
    {
        IQueryable<LionSteel> LionSteels { get; }

        Task<List<LionSteel>> GetListAsync();

        Task<LionSteel> GetByIdAsync(int lionSteelId);
        Task<LionSteel> GetByDOCNoAsync(string docno);

        Task<int> InsertAsync(LionSteel lionSteel);

        Task UpdateAsync(LionSteel lionSteel);

        Task DeleteAsync(LionSteel lionSteel);
    }
}