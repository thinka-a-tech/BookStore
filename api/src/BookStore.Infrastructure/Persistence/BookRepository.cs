using System.Linq.Expressions;
using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence;

public class BookRepository(BookStoreDbContext context) : IRepository<BookEntity, int>
{
    public async Task<BookEntity?> GetByIdAsync(int id) => await context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<BookEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Books.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<PagedResult<BookEntity>> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<BookEntity, object?>> orderBy,
        bool descending,
        CancellationToken cancellationToken = default)
    {
        var query = context.Books.AsNoTracking();
        var total = await query.CountAsync(cancellationToken);

        var ordered = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        // Id tiebreaker keeps paging stable when the sort column has duplicates
        var items = await ordered.ThenBy(b => b.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<BookEntity>(items, page, pageSize, total);
    }

    public async Task AddAsync(BookEntity entity) => await context.Books.AddAsync(entity);

    public void Update(BookEntity entity) => context.Books.Update(entity);

    public void Delete(BookEntity entity) => context.Books.Remove(entity);
}
