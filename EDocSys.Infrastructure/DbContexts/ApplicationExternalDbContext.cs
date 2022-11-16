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
using EDocSys.Domain.Entities.ExternalRecord;
using EDocSys.Domain.Entities.UserMaster;

namespace EDocSys.Infrastructure.DbContexts
{
    public class ApplicationExternalDbContext : AuditableContext, IApplicationExternalDbContext
    {
        private readonly IDateTimeService _dateTime;
        private readonly IAuthenticatedUserService _authenticatedUser;

        public ApplicationExternalDbContext(DbContextOptions<ApplicationExternalDbContext> options, IDateTimeService dateTime, IAuthenticatedUserService authenticatedUser) : base(options)
        {
            _dateTime = dateTime;
            _authenticatedUser = authenticatedUser;
        }

        public DbSet<LionSteel> LionSteels { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
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
