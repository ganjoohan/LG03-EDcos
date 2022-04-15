using EDocSys.Application.Interfaces.CacheRepositories;
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Infrastructure.CacheKeys;
using AspNetCoreHero.Extensions.Caching;
using AspNetCoreHero.ThrowR;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDocSys.Domain.Entities.DocumentationMaster;

namespace EDocSys.Infrastructure.CacheRepositories
{
    public class DepartmentCacheRepository : IDepartmentCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentCacheRepository(IDistributedCache distributedCache, IDepartmentRepository departmentRepository)
        {
            _distributedCache = distributedCache;
            _departmentRepository = departmentRepository;
        }

        public async Task<Department> GetByIdAsync(int departmentId)
        {
            string cacheKey = DepartmentCacheKeys.GetKey(departmentId);
            var department = await _distributedCache.GetAsync<Department>(cacheKey);
            if (department == null)
            {
                department = await _departmentRepository.GetByIdAsync(departmentId);
                Throw.Exception.IfNull(department, "Department", "No Department Found");
                await _distributedCache.SetAsync(cacheKey, department);
            }
            return department;
        }

        public async Task<List<Department>> GetCachedListAsync()
        {
            string cacheKey = DepartmentCacheKeys.ListKey;
            var departmentList = await _distributedCache.GetAsync<List<Department>>(cacheKey);
            if (departmentList == null)
            {
                departmentList = await _departmentRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, departmentList);
            }
            return departmentList;
        }
    }
}
