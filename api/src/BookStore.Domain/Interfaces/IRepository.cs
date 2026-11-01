namespace BookStore.Domain.Interfaces;

public interface IRepository<TEntity, TId> where TEntity : class
{
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}