using BookStore.Domain.Entities;
using BookStore.Domain.Enums;

namespace BookStore.Application.Features.Books;

public sealed record BookDto(int Id, string Title, string Author, string Isbn, int? PublishedYear, string? CoverUrl,
    string OpenLibraryKey, string? Description, Genre Genre, decimal Price, int Stock)
{
    public static BookDto From(BookEntity book)
    {
        return new BookDto(book.Id, book.Title, book.Author, book.Isbn, book.PublishedYear, book.CoverUrl,
            book.OpenLibraryKey, book.Description, book.Genre, book.Price, book.Stock);
    }
}