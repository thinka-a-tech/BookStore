using BookStore.Domain.Entities;
using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces;

public interface IBookReadOnlyRepository : IReadOnlyRepository<BookEntity, int>
{
    Task<IReadOnlyList<GenreCount>> GetGenreCountsAsync(CancellationToken cancellationToken = default);
    Task<bool> IsbnExistsAsync(string isbn, CancellationToken cancellationToken = default);
}
