using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Application.Interfaces.Shared;
using EDocSys.Infrastructure.DbContexts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IAuthenticatedUserService _authenticatedUserService;
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationExternalDbContext _externalDbContext;
        private readonly ApplicationQualityDbContext _qualityDbContext;
        private bool disposed;

        public UnitOfWork(ApplicationDbContext dbContext, ApplicationExternalDbContext externalDbContext, ApplicationQualityDbContext qualityDbContext, IAuthenticatedUserService authenticatedUserService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _externalDbContext = externalDbContext ?? throw new ArgumentNullException(nameof(externalDbContext));
            _qualityDbContext = qualityDbContext ?? throw new ArgumentNullException(nameof(qualityDbContext));
            _authenticatedUserService = authenticatedUserService;
        }

        public async Task<int> Commit(CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task<int> CommitExternal(CancellationToken cancellationToken)
        {
            return await _externalDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> CommitQuality(CancellationToken cancellationToken)
        {
            return await _qualityDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task Rollback()
        {
            //todo
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void DisposeExternal()
        {
            DisposeExternal(true);
            GC.SuppressFinalize(this);
        }
        public void DisposeQuality()
        {
            DisposeQuality(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    _dbContext.Dispose();
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }
        protected virtual void DisposeExternal(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    _externalDbContext.Dispose();
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }
        protected virtual void DisposeQuality(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    _qualityDbContext.Dispose();
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}