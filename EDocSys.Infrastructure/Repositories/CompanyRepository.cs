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
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IRepositoryAsync<Company> _repository;
        private readonly IDistributedCache _distributedCache;

        public CompanyRepository(IDistributedCache distributedCache, IRepositoryAsync<Company> repository)
        {
            _distributedCache = distributedCache;
            _repository = repository;
        }

        public IQueryable<Company> Companies => _repository.Entities;

        public async Task DeleteAsync(Company company)
        {
            await _repository.DeleteAsync(company);
            await _distributedCache.RemoveAsync(CacheKeys.CompanyCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.CompanyCacheKeys.GetKey(company.Id));
        }

        public async Task<Company> GetByIdAsync(int companyId)
        {
            return await _repository.Entities.Where(p => p.Id == companyId).FirstOrDefaultAsync();
        }

        public async Task<List<Company>> GetListAsync()
        {
            return await _repository.Entities.ToListAsync();
        }

        public async Task<int> InsertAsync(Company company)
        {
            await _repository.AddAsync(company);
            await _distributedCache.RemoveAsync(CacheKeys.CompanyCacheKeys.ListKey);
            return company.Id;
        }

        public async Task UpdateAsync(Company company)
        {
            await _repository.UpdateAsync(company);
            await _distributedCache.RemoveAsync(CacheKeys.CompanyCacheKeys.ListKey);
            await _distributedCache.RemoveAsync(CacheKeys.CompanyCacheKeys.GetKey(company.Id));
        }
    }
}
