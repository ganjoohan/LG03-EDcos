using EDocSys.Domain.Entities.Documentation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IEnvironmentalManualRepository
    {
        IQueryable<EnvironmentalManual> EnvironmentalManuals { get; }

        Task<List<EnvironmentalManual>> GetListAsync();

        Task<EnvironmentalManual> GetByIdAsync(int EnvironmentalManualId);
        Task<EnvironmentalManual> GetByDOCNoAsync(string docno);

        Task<int> InsertAsync(EnvironmentalManual EnvironmentalManual);

        Task UpdateAsync(EnvironmentalManual EnvironmentalManual);

        Task DeleteAsync(EnvironmentalManual EnvironmentalManual);
    }
}