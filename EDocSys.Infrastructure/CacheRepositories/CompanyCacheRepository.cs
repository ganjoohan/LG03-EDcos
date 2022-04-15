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
    public class CompanyCacheRepository : ICompanyCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ICompanyRepository _companyRepository;

        public CompanyCacheRepository(IDistributedCache distributedCache, ICompanyRepository companyRepository)
        {
            _distributedCache = distributedCache;
            _companyRepository = companyRepository;
        }

        public async Task<Company> GetByIdAsync(int companyId)
        {
            string cacheKey = CompanyCacheKeys.GetKey(companyId);
            var company = await _distributedCache.GetAsync<Company>(cacheKey);
            if (company == null)
            {
                company = await _companyRepository.GetByIdAsync(companyId);
                Throw.Exception.IfNull(company, "Company", "No Company Found");
                await _distributedCache.SetAsync(cacheKey, company);
            }
            return company;
        }

        public async Task<List<Company>> GetCachedListAsync()
        {
            string cacheKey = CompanyCacheKeys.ListKey;
            var companyList = await _distributedCache.GetAsync<List<Company>>(cacheKey);
            if (companyList == null)
            {
                companyList = await _companyRepository.GetListAsync();
                await _distributedCache.SetAsync(cacheKey, companyList);
            }
            return companyList;
        }
    }
}
