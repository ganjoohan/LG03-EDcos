using AspNetCoreHero.Abstractions.Domain;
using EDocSys.Application.Interfaces.Contexts;
using EDocSys.Application.Interfaces.Shared;
using EDocSys.Domain.Entities.Catalog;
using EDocSys.Domain.Entities.Documentation;
using AspNetCoreHero.EntityFrameworkCore.AuditTrail;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EDocSys.Domain.Entities.DocumentationMaster;
using EDocSys.Domain.Entities.UserMaster;

namespace EDocSys.Infrastructure.DbContexts
{
    public class ApplicationDbContext : AuditableContext, IApplicationDbContext
    {
        private readonly IDateTimeService _dateTime;
        private readonly IAuthenticatedUserService _authenticatedUser;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeService dateTime, IAuthenticatedUserService authenticatedUser) : base(options)
        {
            _dateTime = dateTime;
            _authenticatedUser = authenticatedUser;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<ProcedureStatus> ProcedureStatus { get; set; }
        public DbSet<SOP> StandardOperatingPractices { get; set; }
        public DbSet<WI> WorkInstructions{ get; set; }
        public DbSet<UserApprover> UserApprovers { get; set; }
       
        public DbSet<SOPStatus> SOPStatus { get; set; }

        public DbSet<WIStatus> WIStatus { get; set; }
        public DbSet<DocumentManual> DocumentManuals { get; set; }
        public DbSet<DocumentManualStatus> DocumentManualStatus { get; set; }
        public DbSet<QualityManual> QualityManuals { get; set; }
        public DbSet<QualityManualStatus> QualityManualStatus { get; set; }
        public DbSet<EnvironmentalManual> EnvironmentalManuals { get; set; }
        public DbSet<EnvironmentalManualStatus> EnvironmentalManualStatus { get; set; }
        public DbSet<LabAccreditationManual> LabAccreditationManuals { get; set; }
        public DbSet<LabAccreditationManualStatus> LabAccreditationManualStatus { get; set; }
        public DbSet<SafetyHealthManual> SafetyHealthManuals { get; set; }
        public DbSet<SafetyHealthManualStatus> SafetyHealthManualStatus { get; set; }
        public DbSet<Issuance> Issuances { get; set; }
        public DbSet<IssuanceInfo> IssuancesInfo { get; set; }
        public DbSet<IssuanceInfoPrint> IssuancesInfoPrint { get; set; }
        public DbSet<IssuanceStatus> IssuanceStatus { get; set; }
        public IDbConnection Connection => Database.GetDbConnection();

        public bool HasChanges => ChangeTracker.HasChanges();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = _dateTime.NowUtc;
                        entry.Entity.CreatedBy = _authenticatedUser.UserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = _dateTime.NowUtc;
                        entry.Entity.LastModifiedBy = _authenticatedUser.UserId;
                        break;
                }
            }
            if (_authenticatedUser.UserId == null)
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            else
            {
                return await base.SaveChangesAsync(_authenticatedUser.UserId);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }
            base.OnModelCreating(builder);
        }
    }
}