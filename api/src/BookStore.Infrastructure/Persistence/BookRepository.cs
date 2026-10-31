using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence;

public class BookRepository(BookStoreDbContext context) : IRepository<BookEntity, int>
{
    public async Task<BookEntity?> GetByIdAsync(int id) => await context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<BookEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Books.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(BookEntity entity) => await context.Books.AddAsync(entity);

    public void Update(BookEntity entity) => context.Books.Update(entity);

    public void Delete(BookEntity entity) => context.Books.Remove(entity);
}
