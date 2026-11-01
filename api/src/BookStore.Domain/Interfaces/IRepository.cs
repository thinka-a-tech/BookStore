namespace BookStore.Domain.Interfaces;

public interface IRepository<TEntity, TId> where TEntity : class
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}