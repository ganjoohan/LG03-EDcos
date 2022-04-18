using EDocSys.Application.Interfaces.CacheRepositories;
using EDocSys.Application.Interfaces.Contexts;
using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Infrastructure.CacheRepositories;
using EDocSys.Infrastructure.DbContexts;
using EDocSys.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EDocSys.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistenceContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            #region Repositories

            services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProductCacheRepository, ProductCacheRepository>();
            services.AddTransient<IBrandRepository, BrandRepository>();
            services.AddTransient<IBrandCacheRepository, BrandCacheRepository>();
            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IDepartmentCacheRepository, DepartmentCacheRepository>();

            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<ICompanyCacheRepository, CompanyCacheRepository>();

            services.AddTransient<IProcedureRepository, ProcedureRepository>();
            services.AddTransient<IProcedureCacheRepository, ProcedureCacheRepository>();

            services.AddTransient<ISOPRepository, SOPRepository>();
            services.AddTransient<ISOPCacheRepository, SOPCacheRepository>();

            services.AddTransient<IWIRepository, WIRepository>();
            services.AddTransient<IWICacheRepository, WICacheRepository>();

            services.AddTransient<IProcedureStatusRepository, ProcedureStatusRepository>();
            services.AddTransient<IProcedureStatusCacheRepository, ProcedureStatusCacheRepository>();

            services.AddTransient<IWIStatusRepository, WIStatusRepository>();
            services.AddTransient<IWIStatusCacheRepository, WIStatusCacheRepository>();

            services.AddTransient<IUserApproverRepository, UserApproverRepository>();
            services.AddTransient<IUserApproverCacheRepository, UserApproverCacheRepository>();

            #endregion Repositories
        }
    }
}