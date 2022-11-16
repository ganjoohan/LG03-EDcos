using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDocSys.Application.Interfaces.Repositories
{
    public interface IRepositoryAsync<T> where T : class
    {
        IQueryable<T> Entities { get; }

        Task<T> GetByIdAsync(int id);

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetPagedReponseAsync(int pageNumber, int pageSize);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        IQueryable<T> EntitiesExternal { get; }

        Task<T> GetByIdExternalAsync(int id);

        Task<List<T>> GetAllExternalAsync();

        Task<List<T>> GetPagedReponseExternalAsync(int pageNumber, int pageSize);

        Task<T> AddExternalAsync(T entity);

        Task UpdateExternalAsync(T entity);

        Task DeleteExternalAsync(T entity);

        IQueryable<T> EntitiesQuality { get; }

        Task<T> GetByIdQualityAsync(int id);

        Task<List<T>> GetAllQualityAsync();

        Task<List<T>> GetPagedReponseQualityAsync(int pageNumber, int pageSize);

        Task<T> AddQualityAsync(T entity);

        Task UpdateQualityAsync(T entity);

        Task DeleteQualityAsync(T entity);
    }
}