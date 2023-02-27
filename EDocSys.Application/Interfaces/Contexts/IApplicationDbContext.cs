using EDocSys.Domain.Entities.Catalog;
using EDocSys.Domain.Entities.Documentation;
using EDocSys.Domain.Entities.DocumentationMaster;
using EDocSys.Domain.Entities.UserMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Contexts
{
    public interface IApplicationDbContext
    {
        IDbConnection Connection { get; }
        bool HasChanges { get; }

        EntityEntry Entry(object entity);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        DbSet<Product> Products { get; set; }
        DbSet<Procedure> Procedures { get; set; }
        DbSet<SOP> StandardOperatingPractices { get; set; }
        DbSet<WI> WorkInstructions { get; set; }
        DbSet<DocumentManual> DocumentManuals { get; set; }
        DbSet<QualityManual> QualityManuals { get; set; }
        DbSet<EnvironmentalManual> EnvironmentalManuals { get; set; }
        DbSet<LabAccreditationManual> LabAccreditationManuals { get; set; }
        DbSet<SafetyHealthManual> SafetyHealthManuals { get; set; }
        DbSet<UserApprover> UserApprovers { get; set; }
        DbSet<Issuance> Issuances { get; set; }
        DbSet<IssuanceInfo> IssuancesInfo { get; set; }
    }
}