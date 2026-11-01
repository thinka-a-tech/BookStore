using System.Linq.Expressions;
using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces;

public interface IReadOnlyRepository<TEntity, TId> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> GetAllPagedAsync(int page, int pageSize,
        Expression<Func<TEntity, object?>> orderBy, bool descending,
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default);
}