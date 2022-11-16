using EDocSys.Domain.Entities.QualityRecord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Contexts
{
    public interface IApplicationQualityDbContext
    {
        IDbConnection Connection { get; }
        bool HasChanges { get; }

        EntityEntry Entry(object entity);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        DbSet<LionSteel> LionSteels { get; set; }
        DbSet<Attachment> Attachments { get; set; }


    }
}
