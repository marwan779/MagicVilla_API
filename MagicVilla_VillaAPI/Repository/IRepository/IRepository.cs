using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? Filter = null, string? IncludeProperties = null,
            int PageSize = 0, int PageNumber = 1);

        Task<T> GetAsync(Expression<Func<T, bool>>? Filter = null, string? IncludeProperties = null,
            bool Tracked = true); 
        
        Task CreateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveAsync();
    }
}
