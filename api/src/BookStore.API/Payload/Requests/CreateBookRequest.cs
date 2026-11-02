using BookStore.Domain.Enums;

namespace BookStore.API.Payload.Requests;

public sealed record CreateBookRequest(string Title, string Author, string Isbn, string OpenLibraryKey, Genre Genre,
    decimal Price, int Stock = 0, int? PublishedYear = null, string? CoverUrl = null, string? Description = null);
