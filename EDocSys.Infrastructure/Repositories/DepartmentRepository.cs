using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Domain.Entities.DocumentationMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IRepositoryAsync<Department> _repository;
        private readonly IDistributedCache _distributedCache;

        public DepartmentRepository(IDistributedCache distributedCache, IRepositoryAsync<Department> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<Department> Departments => _repository.Entities;

        public async Task DeleteAsync(Department department)
        {
            await _repository.DeleteAsync(department);
            await _distributedCache.RemoveAsync(CacheKeys.DepartmentCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.DepartmentCacheKeys.GetKey(department.Id));
        }

        public async Task<Department> GetByIdAsync(int departmentId)
        {
            return await _repository.Entities.Where(p => p.Id == departmentId).FirstOrDefaultAsync();
        }

        public async Task<List<Department>> GetListAsync()
        {
            return await _repository.Entities.ToListAsync();
        }

        public async Task<int> InsertAsync(Department department)
        {
            await _repository.AddAsync(department);
            await _distributedCache.RemoveAsync(CacheKeys.DepartmentCacheKeys.ListKey);
            return department.Id;
        }

        public async Task UpdateAsync(Department department)
        {
            await _repository.UpdateAsync(department);
            await _distributedCache.RemoveAsync(CacheKeys.DepartmentCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.DepartmentCacheKeys.GetKey(department.Id));
        }
    }
}
