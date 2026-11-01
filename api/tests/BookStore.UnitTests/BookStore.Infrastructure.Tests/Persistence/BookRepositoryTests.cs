using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Models;
using BookStore.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BookStore.UnitTests.BookStore.Infrastructure.Tests.Persistence;

public class BookRepositoryTests : IDisposable
{
    private readonly SqliteConnection connection;
    private readonly BookStoreDbContext context;
    private readonly BookRepository repository;

    public BookRepositoryTests()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<BookStoreDbContext>().UseSqlite(connection).Options;

        context = new BookStoreDbContext(options);
        context.Database.EnsureCreated();

        repository = new BookRepository(context);
    }

    [Fact]
    public async Task GetAllPagedAsync_FiltersBeforeCountingAndPagesWithStableTieBreak()
    {
        //Arrange
        var first = CreateBook("SampleTitle", Genre.SciFi, 10m, "1");
        var second = CreateBook("SampleTitle", Genre.SciFi, 20m, "2");
        var third = CreateBook("DifferentTitle", Genre.SciFi, 30m, "3");

        context.Books.AddRange(first, second, third, CreateBook("AnotherTitle", Genre.Fantasy, 40m, "4"));

        await context.SaveChangesAsync();

        //Act
        var page = await repository.GetAllPagedAsync(2, 1, b => b.Title, false, b => b.Genre == Genre.SciFi);
        var nextPage = await repository.GetAllPagedAsync(3, 1, b => b.Title, false, b => b.Genre == Genre.SciFi);

        //Assert
        Assert.Equal(3, page.TotalCount);
        Assert.Equal(3, page.TotalPages);
        Assert.Equal(2, page.Page);
        Assert.Equal(1, page.PageSize);
        Assert.Equal(first.Id, Assert.Single(page.Items).Id);
        Assert.Equal(second.Id, Assert.Single(nextPage.Items).Id);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetAllPagedAsync_SortsByPriceInRequestedDirection(bool descending)
    {
        //Arrange
        context.Books.AddRange(
            CreateBook("Middle", Genre.Fiction, 10m, "1"),
            CreateBook("Expensive", Genre.Fiction, 20m, "2"),
            CreateBook("Cheap", Genre.Fiction, 2m, "3"));

        await context.SaveChangesAsync();

        var expected = descending
            ? new[] { "Expensive", "Middle", "Cheap" }
            : new[] { "Cheap", "Middle", "Expensive" };

        //Act
        var page = await repository.GetAllPagedAsync(1, 10, b => b.Price, descending);

        //Assert
        Assert.Equal(expected, page.Items.Select(b => b.Title));
    }

    [Fact]
    public async Task GetGenreCountsAsync_GroupsAndOrdersGenres()
    {
        //Arrange
        context.Books.AddRange(
            CreateBook("One", Genre.SciFi, 1m, "1"),
            CreateBook("Two", Genre.SciFi, 2m, "2"),
            CreateBook("Three", Genre.Fantasy, 3m, "3"));

        await context.SaveChangesAsync();

        //Act
        var counts = await repository.GetGenreCountsAsync();

        //Assert
        IReadOnlyList<GenreCount> expected = [new GenreCount(Genre.Fantasy, 1), new GenreCount(Genre.SciFi, 2)];
        Assert.Equal(expected, counts);
    }

    [Fact]
    public async Task GetGenreCountsAsync_WithoutBooks_ReturnsEmptyList()
    {
        //Arrange
        // The constructor creates an empty database.

        //Act
        var counts = await repository.GetGenreCountsAsync();

        //Assert
        Assert.Empty(counts);
    }

    private static BookEntity CreateBook(string title, Genre genre, decimal price, string key)
    {
        return new BookEntity(title, "Author", key, $"/works/{key}", genre, price);
    }

    public void Dispose()
    {
        context.Dispose();
        connection.Dispose();
    }
}
