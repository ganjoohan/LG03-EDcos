using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Domain.Entities.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class ProcedureRepository : IProcedureRepository
    {
        private readonly IRepositoryAsync<Procedure> _repository;
        private readonly IDistributedCache _distributedCache;

        public ProcedureRepository(IDistributedCache distributedCache, IRepositoryAsync<Procedure> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<Procedure> Procedures => _repository.Entities;

        public async Task DeleteAsync(Procedure procedure)
        {
            await _repository.DeleteAsync(procedure);
            await _distributedCache.RemoveAsync(CacheKeys.ProcedureCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.ProcedureCacheKeys.GetKey(procedure.Id));
        }

        public async Task<Procedure> GetByIdAsync(int procedureId)
        {
            return await _repository.Entities.Where(p => p.Id == procedureId)
                .Include(a => a.Department)
                .Include(a => a.Company)
                .FirstOrDefaultAsync();
        }

        public async Task<Procedure> GetByWSCPNoAsync(string wscpno)
        {
            return await _repository.Entities.Where(p => p.WSCPNo == wscpno)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Procedure>> GetByParameterAsync(int companyId, int departmentId)
        {
            return _repository.Entities.Where(p => p.CompanyId == companyId && p.DepartmentId == departmentId && p.IsActive == true)
               .ToList();
        }

        public async Task<List<Procedure>> GetListAsync()
        {
            return _repository.Entities
               .Include(a => a.Department)
               .Include(a => a.Company)
               .ToList();
            //big data, slow loading (Elaine Ho 5Aug2022)
            //return await _repository.Entities
            //    .Include(a => a.Department)
            //    .Include(a => a.Company)
            //    .ToListAsync();
        }

        public async Task<int> InsertAsync(Procedure procedure)
        {
            await _repository.AddAsync(procedure);
            await _distributedCache.RemoveAsync(CacheKeys.ProcedureCacheKeys.ListKey);
            return procedure.Id;
        }

        public async Task UpdateAsync(Procedure procedure)
        {
            await _repository.UpdateAsync(procedure);
            await _distributedCache.RemoveAsync(CacheKeys.ProcedureCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.ProcedureCacheKeys.GetKey(procedure.Id));
        }
    }
}