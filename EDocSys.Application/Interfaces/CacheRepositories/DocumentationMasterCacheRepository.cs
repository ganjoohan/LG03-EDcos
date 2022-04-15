using EDocSys.Domain.Entities.DocumentationMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.CacheRepositories
{
    public interface IDepartmentCacheRepository
    {
        Task<List<Department>> GetCachedListAsync();

        Task<Department> GetByIdAsync(int departmentId);
    }

    public interface ICompanyCacheRepository
    {
        Task<List<Company>> GetCachedListAsync();

        Task<Company> GetByIdAsync(int cityId);
    }
}
