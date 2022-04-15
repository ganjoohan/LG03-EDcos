using EDocSys.Domain.Entities.Documentation;
using EDocSys.Domain.Entities.DocumentationMaster;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface ICompanyRepository
    {
        IQueryable<Company> Companies { get; }

        Task<List<Company>> GetListAsync();

        Task<Company> GetByIdAsync(int companyId);

        Task<int> InsertAsync(Company company);

        Task UpdateAsync(Company company);

        Task DeleteAsync(Company company);
    }
}