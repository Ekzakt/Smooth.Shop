using System.Linq.Expressions;

namespace Smooth.Shop.Application.Contracts;

public interface IGenericRepository<T>
{
    Task AddAsync(T entity);

    Task DeleteAsync(long id);

    Task<IEnumerable<T>> GetAllAsync();

    Task<T?> GetByIdAsync(long id);

    Task<IEnumerable<T>> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Expression<Func<T, bool>>? filter = null, 
        Expression<Func<T, object>>? orderBy = null,
        bool orderByDescending = false);

    Task UpdateAsync(T entity);
}