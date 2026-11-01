using System.Linq.Expressions;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence;

public class BookRepository(BookStoreDbContext context) : IRepository<BookEntity, int>, IBookReadOnlyRepository
{
    public async Task<BookEntity?> GetByIdAsync(int id) => await context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<BookEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Books.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<PagedResult<BookEntity>> GetAllPagedAsync(
        int page,
        int pageSize,
        Expression<Func<BookEntity, object?>> orderBy,
        bool descending,
        Expression<Func<BookEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Books.AsNoTracking();
        if (filter is not null)
        {
            query = query.Where(filter);
        }

        // Get total
        var total = await query.CountAsync(cancellationToken);

        var ordered = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        // Id tiebreaker keeps paging stable when the sort column has duplicates
        var items = await ordered.ThenBy(b => b.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<BookEntity>(items, page, pageSize, total);
    }

    // Hand-written SQL: books grouped by genre with a count per genre
    public async Task<IReadOnlyList<GenreCount>> GetGenreCountsAsync(CancellationToken cancellationToken = default)
    {
        var result = await context.Database
            .SqlQuery<GenreCountRow>($"SELECT Genre, COUNT(*) AS Count FROM Books GROUP BY Genre ORDER BY Genre")
            .ToListAsync(cancellationToken);

        return result.Select(r => new GenreCount(Enum.Parse<Genre>(r.Genre), r.Count)).ToList();
    }

    public async Task AddAsync(BookEntity entity) => await context.Books.AddAsync(entity);

    public void Update(BookEntity entity) => context.Books.Update(entity);

    public void Delete(BookEntity entity) => context.Books.Remove(entity);

    private sealed record GenreCountRow(string Genre, int Count);
}
