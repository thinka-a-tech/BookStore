using System.Linq.Expressions;
using BookStore.Application.Features.Books.GetBooks;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

namespace BookStore.UnitTests.BookStore.Application.Tests.Features.Books.GetBooks;

public class GetBooksHandlerTests
{
    [Fact]
    public async Task Handle_PassesPagingParameters()
    {
        //Arrange
        var repository = new RecordingRepository(new PagedResult<BookEntity>([], 2, 5, 12));
        var handler = new GetBooksQueryHandler(repository);

        //Act
        await handler.Handle(new GetBooksQuery(Page: 2, PageSize: 5), CancellationToken.None);

        //Assert
        Assert.Equal(2, repository.Page);
        Assert.Equal(5, repository.PageSize);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_PassesSortDirection(bool descending)
    {
        //Arrange
        var repository = new RecordingRepository(new PagedResult<BookEntity>([], 1, 10, 0));
        var handler = new GetBooksQueryHandler(repository);

        //Act
        await handler.Handle(new GetBooksQuery(Descending: descending), CancellationToken.None);

        //Assert
        Assert.Equal(descending, repository.Descending);
    }

    [Fact]
    public async Task Handle_PassesCancellationToken()
    {
        //Arrange
        var repository = new RecordingRepository(new PagedResult<BookEntity>([], 1, 10, 0));
        var handler = new GetBooksQueryHandler(repository);
        using var cancellation = new CancellationTokenSource();

        //Act
        await handler.Handle(new GetBooksQuery(), cancellation.Token);

        //Assert
        Assert.Equal(cancellation.Token, repository.CancellationToken);
    }

    [Fact]
    public async Task Handle_FiltersByRequestedGenre()
    {
        //Arrange
        var repository = new RecordingRepository(new PagedResult<BookEntity>([], 1, 10, 0));
        var handler = new GetBooksQueryHandler(repository);

        //Act
        await handler.Handle(new GetBooksQuery(Genre: Genre.SciFi), CancellationToken.None);

        //Assert
        Assert.NotNull(repository.Filter);
        var filter = repository.Filter.Compile();
        Assert.True(filter(CreateBook(Genre.SciFi)));
        Assert.False(filter(CreateBook(Genre.Fantasy)));
    }

    [Fact]
    public async Task Handle_PreservesPagingMetadata()
    {
        //Arrange
        var repository = new RecordingRepository(new PagedResult<BookEntity>([], 2, 5, 12));
        var handler = new GetBooksQueryHandler(repository);

        //Act
        var result = await handler.Handle(new GetBooksQuery(Page: 2, PageSize: 5), CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        var page = result.Value;
        Assert.Equal(2, page.Page);
        Assert.Equal(5, page.PageSize);
        Assert.Equal(12, page.TotalCount);
        Assert.Equal(3, page.TotalPages);
    }

    [Fact]
    public async Task Handle_MapsBookToDto()
    {
        //Arrange
        var book = CreateBook(Genre.SciFi);
        var repository = new RecordingRepository(new PagedResult<BookEntity>([book], 1, 10, 1));
        var handler = new GetBooksQueryHandler(repository);

        //Act
        var result = await handler.Handle(new GetBooksQuery(), CancellationToken.None);

        //Assert
        var dto = Assert.Single(result.Value.Items);

        Assert.True(result.IsSuccess);
        Assert.Equal(book.Id, dto.Id);
        Assert.Equal(book.Title, dto.Title);
        Assert.Equal(book.Author, dto.Author);
        Assert.Equal(book.Isbn, dto.Isbn);
        Assert.Equal(book.OpenLibraryKey, dto.OpenLibraryKey);
        Assert.Equal(book.Genre, dto.Genre);
        Assert.Equal(book.Price, dto.Price);
        Assert.Equal(book.Stock, dto.Stock);
        Assert.Equal(book.PublishedYear, dto.PublishedYear);
        Assert.Equal(book.CoverUrl, dto.CoverUrl);
        Assert.Equal(book.Description, dto.Description);
    }

    [Theory]
    [InlineData(BookSortField.Title)]
    [InlineData(BookSortField.Author)]
    [InlineData(BookSortField.Genre)]
    [InlineData(BookSortField.Price)]
    [InlineData(BookSortField.PublishedYear)]
    [InlineData(BookSortField.Stock)]
    public async Task Handle_SelectsSortField_WithoutGenreFilter(BookSortField sortField)
    {
        //Arrange
        var book = CreateBook(Genre.SciFi);
        var repository = new RecordingRepository(new PagedResult<BookEntity>([], 1, 10, 0));
        var handler = new GetBooksQueryHandler(repository);
        object? expected = sortField switch
        {
            BookSortField.Title => book.Title,
            BookSortField.Author => book.Author,
            BookSortField.Genre => book.Genre,
            BookSortField.Price => book.Price,
            BookSortField.PublishedYear => book.PublishedYear,
            BookSortField.Stock => book.Stock,
            _ => throw new ArgumentOutOfRangeException(nameof(sortField))
        };

        //Act
        var result = await handler.Handle(new GetBooksQuery(SortBy: sortField), CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Null(repository.Filter);
        Assert.Empty(result.Value.Items);
        Assert.Equal(expected, repository.OrderBy!.Compile()(book));
    }

    private static BookEntity CreateBook(Genre genre)
    {
        return new BookEntity("Title", "Author", "123", "/works/1", genre, 12.50m, 3, 2020, "cover", "description");
    }

    private sealed class RecordingRepository(PagedResult<BookEntity> result) : IReadOnlyRepository<BookEntity, int>
    {
        public int Page { get; private set; }
        public int PageSize { get; private set; }
        public bool Descending { get; private set; }
        public Expression<Func<BookEntity, object?>>? OrderBy { get; private set; }
        public Expression<Func<BookEntity, bool>>? Filter { get; private set; }
        public CancellationToken CancellationToken { get; private set; }

        public Task<PagedResult<BookEntity>> GetAllPagedAsync(
            int page, int pageSize, Expression<Func<BookEntity, object?>> orderBy, bool descending,
            Expression<Func<BookEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            Page = page;
            PageSize = pageSize;
            OrderBy = orderBy;
            Descending = descending;
            Filter = filter;
            CancellationToken = cancellationToken;
            return Task.FromResult(result);
        }

        // Not used for now
        public Task<BookEntity?> GetByIdAsync(int id) => throw new NotSupportedException();

        // Not used for now
        public Task<IEnumerable<BookEntity>> GetAllAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
