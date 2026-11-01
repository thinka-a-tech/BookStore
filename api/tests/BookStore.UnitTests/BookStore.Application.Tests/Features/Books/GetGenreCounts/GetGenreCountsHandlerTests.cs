using System.Linq.Expressions;
using BookStore.Application.Features.Books.GetGenreCounts;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

namespace BookStore.UnitTests.BookStore.Application.Tests.Features.Books.GetGenreCounts;

public class GetGenreCountsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCounts()
    {
        //Arrange
        IReadOnlyList<GenreCount> counts = [new(Genre.Fantasy, 3), new(Genre.SciFi, 2)];
        var repository = new GenreCountsRepository(counts);
        var handler = new GetGenreCountsHandler(repository);

        //Act
        var result = await handler.Handle(new GetGenreCountsQuery(), CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(counts, result.Value);
    }

    [Fact]
    public async Task Handle_PassesCancellationToken()
    {
        //Arrange
        var repository = new GenreCountsRepository([]);
        var handler = new GetGenreCountsHandler(repository);
        using var cancellation = new CancellationTokenSource();

        //Act
        await handler.Handle(new GetGenreCountsQuery(), cancellation.Token);

        //Assert
        Assert.Equal(cancellation.Token, repository.CancellationToken);
    }

    private sealed class GenreCountsRepository(IReadOnlyList<GenreCount> counts) : IBookReadOnlyRepository
    {
        public CancellationToken CancellationToken { get; private set; }

        public Task<IReadOnlyList<GenreCount>> GetGenreCountsAsync(CancellationToken cancellationToken = default)
        {
            CancellationToken = cancellationToken;
            return Task.FromResult(counts);
        }

        public Task<BookEntity?> GetByIdAsync(int id) => throw new NotSupportedException();
        public Task<IEnumerable<BookEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
        public Task<PagedResult<BookEntity>> GetAllPagedAsync(
            int page, int pageSize, Expression<Func<BookEntity, object?>> orderBy, bool descending,
            Expression<Func<BookEntity, bool>>? filter = null, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
