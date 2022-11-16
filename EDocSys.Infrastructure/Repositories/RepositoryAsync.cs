using EDocSys.Application.Interfaces.Repositories;
using EDocSys.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Infrastructure.Repositories
{
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationExternalDbContext _externalDbContext;
        private readonly ApplicationQualityDbContext _qualityDbContext;

        public RepositoryAsync(ApplicationDbContext dbContext, ApplicationExternalDbContext externalDbContext, ApplicationQualityDbContext qualityDbContext)
        {
            _dbContext = dbContext;
            _externalDbContext = externalDbContext;
            _qualityDbContext = qualityDbContext;
        }

        public IQueryable<T> Entities => _dbContext.Set<T>();
        public IQueryable<T> EntitiesExternal => _externalDbContext.Set<T>();
        public IQueryable<T> EntitiesQuality => _qualityDbContext.Set<T>();

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext
                .Set<T>()
                .ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetPagedReponseAsync(int pageNumber, int pageSize)
        {
            return await _dbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(entity);
            return Task.CompletedTask;
        }

        public async Task<T> AddExternalAsync(T entity)
        {
            await _externalDbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task DeleteExternalAsync(T entity)
        {
            _externalDbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<List<T>> GetAllExternalAsync()
        {
            return await _externalDbContext
                .Set<T>()
                .ToListAsync();
        }

        public async Task<T> GetByIdExternalAsync(int id)
        {
            return await _externalDbContext.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetPagedReponseExternalAsync(int pageNumber, int pageSize)
        {
            return await _externalDbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task UpdateExternalAsync(T entity)
        {
            _externalDbContext.Entry(entity).CurrentValues.SetValues(entity);
            return Task.CompletedTask;
        }

        public async Task<T> AddQualityAsync(T entity)
        {
            await _qualityDbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public Task DeleteQualityAsync(T entity)
        {
            _qualityDbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<List<T>> GetAllQualityAsync()
        {
            return await _qualityDbContext
                .Set<T>()
                .ToListAsync();
        }

        public async Task<T> GetByIdQualityAsync(int id)
        {
            return await _qualityDbContext.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetPagedReponseQualityAsync(int pageNumber, int pageSize)
        {
            return await _qualityDbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task UpdateQualityAsync(T entity)
        {
            _qualityDbContext.Entry(entity).CurrentValues.SetValues(entity);
            return Task.CompletedTask;
        }
    }
}