using System.Linq.Expressions;
using BookStore.Domain.Common;

namespace BookStore.Domain.Interfaces;

public interface IReadOnlyRepository<TEntity, TId> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> GetPagedAsync(int page, int pageSize,
        Expression<Func<TEntity, object?>> orderBy, bool descending,
        CancellationToken cancellationToken = default);
}