namespace BookStore.Application.Features.Catalog;

public sealed record CatalogBookDto(string Title, string Author, string? Isbn, int? PublishedYear, string? CoverUrl, string OpenLibraryKey);
