using System;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> Commit(CancellationToken cancellationToken);
        Task<int> CommitExternal(CancellationToken cancellationToken);
        Task<int> CommitQuality(CancellationToken cancellationToken);

        Task Rollback();
    }
}