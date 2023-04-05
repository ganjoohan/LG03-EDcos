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
using EDocSys.Application.Interfaces.Repositories.ExternalRepositories;
using EDocSys.Infrastructure.Repositories.ExternalRepositories;
using EDocSys.Infrastructure.CacheRepositories.ExternalCacheRepositories;
using EDocSys.Application.Interfaces.CacheRepositories.ExternalCacheRepositories;
using EDocSys.Application.Interfaces.Repositories.QualityRepositories;
using EDocSys.Infrastructure.Repositories.QualityRepositories;
using EDocSys.Infrastructure.CacheRepositories.QualityCacheRepositories;
using EDocSys.Application.Interfaces.CacheRepositories.QualityCacheRepositories;

namespace EDocSys.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistenceContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IApplicationExternalDbContext, ApplicationExternalDbContext>();
            services.AddScoped<IApplicationQualityDbContext, ApplicationQualityDbContext>();
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

            services.AddTransient<ISOPStatusRepository, SOPStatusRepository>();
            services.AddTransient<ISOPStatusCacheRepository, SOPStatusCacheRepository>();

            services.AddTransient<IWIRepository, WIRepository>();
            services.AddTransient<IWICacheRepository, WICacheRepository>();

            services.AddTransient<IDocumentManualRepository, DocumentManualRepository>();
            services.AddTransient<IDocumentManualCacheRepository, DocumentManualCacheRepository>();

            services.AddTransient<IQualityManualRepository, QualityManualRepository>();
            services.AddTransient<IQualityManualCacheRepository, QualityManualCacheRepository>();

            services.AddTransient<IEnvironmentalManualRepository, EnvironmentalManualRepository>();
            services.AddTransient<IEnvironmentalManualCacheRepository, EnvironmentalManualCacheRepository>();

            services.AddTransient<ILabAccreditationManualRepository, LabAccreditationManualRepository>();
            services.AddTransient<ILabAccreditationManualCacheRepository, LabAccreditationManualCacheRepository>();

            services.AddTransient<ISafetyHealthManualRepository, SafetyHealthManualRepository>();
            services.AddTransient<ISafetyHealthManualCacheRepository, SafetyHealthManualCacheRepository>();

            services.AddTransient<IProcedureStatusRepository, ProcedureStatusRepository>();
            services.AddTransient<IProcedureStatusCacheRepository, ProcedureStatusCacheRepository>();

            services.AddTransient<IWIStatusRepository, WIStatusRepository>();
            services.AddTransient<IWIStatusCacheRepository, WIStatusCacheRepository>();

            services.AddTransient<IDocumentManualStatusRepository, DocumentManualStatusRepository>();
            services.AddTransient<IDocumentManualStatusCacheRepository, DocumentManualStatusCacheRepository>();

            services.AddTransient<IQualityManualStatusRepository, QualityManualStatusRepository>();
            services.AddTransient<IQualityManualStatusCacheRepository, QualityManualStatusCacheRepository>();

            services.AddTransient<IEnvironmentalManualStatusRepository, EnvironmentalManualStatusRepository>();
            services.AddTransient<IEnvironmentalManualStatusCacheRepository, EnvironmentalManualStatusCacheRepository>();

            services.AddTransient<ILabAccreditationManualStatusRepository, LabAccreditationManualStatusRepository>();
            services.AddTransient<ILabAccreditationManualStatusCacheRepository, LabAccreditationManualStatusCacheRepository>();

            services.AddTransient<ISafetyHealthManualStatusRepository, SafetyHealthManualStatusRepository>();
            services.AddTransient<ISafetyHealthManualStatusCacheRepository, SafetyHealthManualStatusCacheRepository>();

            services.AddTransient<IUserApproverRepository, UserApproverRepository>();
            services.AddTransient<IUserApproverCacheRepository, UserApproverCacheRepository>();

            services.AddTransient<Application.Interfaces.Repositories.ExternalRepositories.ILionSteelRepository, Repositories.ExternalRepositories.LionSteelRepository>();
            services.AddTransient<Application.Interfaces.CacheRepositories.ExternalCacheRepositories.ILionSteelCacheRepository, CacheRepositories.ExternalCacheRepositories.LionSteelCacheRepository>();

            services.AddTransient<Application.Interfaces.Repositories.QualityRepositories.ILionSteelRepository, Repositories.QualityRepositories.LionSteelRepository>();
            services.AddTransient<Application.Interfaces.CacheRepositories.QualityCacheRepositories.ILionSteelCacheRepository, CacheRepositories.QualityCacheRepositories.LionSteelCacheRepository>();

            services.AddTransient<Application.Interfaces.Repositories.ExternalRepositories.IAttachmentRepository, Repositories.ExternalRepositories.AttachmentRepository> ();
            services.AddTransient<Application.Interfaces.CacheRepositories.ExternalCacheRepositories.IAttachmentCacheRepository, CacheRepositories.ExternalCacheRepositories.AttachmentCacheRepository>();

            services.AddTransient<Application.Interfaces.Repositories.QualityRepositories.IAttachmentRepository, Repositories.QualityRepositories.AttachmentRepository>();
            services.AddTransient<Application.Interfaces.CacheRepositories.QualityCacheRepositories.IAttachmentCacheRepository, CacheRepositories.QualityCacheRepositories.AttachmentCacheRepository>();

            services.AddTransient<IIssuanceRepository, IssuanceRepository>();
            services.AddTransient<IIssuanceCacheRepository, IssuanceCacheRepository>();

            services.AddTransient<IIssuanceInfoRepository, IssuanceInfoRepository>();
            services.AddTransient<IIssuanceInfoCacheRepository, IssuanceInfoCacheRepository>();

            services.AddTransient<IIssuanceStatusRepository, IssuanceStatusRepository>();
            services.AddTransient<IIssuanceStatusCacheRepository, IssuanceStatusCacheRepository>();

            services.AddTransient<IIssuanceInfoPrintRepository, IssuanceInfoPrintRepository>();
            services.AddTransient<IIssuanceInfoPrintCacheRepository, IssuanceInfoPrintCacheRepository>();

            #endregion Repositories
        }
    }
}