using EDocSys.Domain.Entities.Documentation;
using EDocSys.Domain.Entities.DocumentationMaster;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository
    {
        IQueryable<Department> Departments { get; }

        Task<List<Department>> GetListAsync();

        Task<Department> GetByIdAsync(int departmentId);

        Task<int> InsertAsync(Department department);

        Task UpdateAsync(Department department);

        Task DeleteAsync(Department department);
    }
}