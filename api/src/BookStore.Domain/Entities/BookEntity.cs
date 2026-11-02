using BookStore.Domain.Enums;

namespace BookStore.Domain.Entities;

public class BookEntity
{
    public int Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public string Isbn { get; private set; } = string.Empty;
    public string OpenLibraryKey { get; private set; } = string.Empty;
    public Genre Genre { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public int? PublishedYear { get; private set; }
    public string? CoverUrl { get; private set; }
    public string? Description { get; private set; }

    private BookEntity() { }

    public BookEntity(string title, string author, string isbn, string openLibraryKey, Genre genre, decimal price, int stock = 0, int? publishedYear = null, string? coverUrl = null, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentOutOfRangeException.ThrowIfNegative(price);
        ArgumentOutOfRangeException.ThrowIfNegative(stock);

        Title = title;
        Author = author;
        Isbn = isbn;
        OpenLibraryKey = openLibraryKey;
        Genre = genre;
        Price = price;
        Stock = stock;
        PublishedYear = publishedYear;
        CoverUrl = coverUrl;
        Description = description;
    }

    public void UpdateBook(Genre genre, decimal price, int stock, string? description)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(price);
        ArgumentOutOfRangeException.ThrowIfNegative(stock);

        Genre = genre;
        Price = price;
        Stock = stock;
        Description = description;
    }
}
