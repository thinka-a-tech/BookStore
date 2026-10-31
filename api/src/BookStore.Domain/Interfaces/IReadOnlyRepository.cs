namespace BookStore.Domain.Interfaces;

public interface IReadOnlyRepository<TEntity, TId> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}