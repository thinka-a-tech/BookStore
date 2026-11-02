using System.Text.Json.Serialization;

namespace BookStore.Infrastructure.ExternalServices.OpenLibrary;

internal sealed record OpenLibrarySearchResponse(
    [property: JsonPropertyName("numFound")] int NumFound,
    [property: JsonPropertyName("docs")] List<OpenLibraryDoc>? Docs);

internal sealed record OpenLibraryDoc(
    [property: JsonPropertyName("key")] string? Key,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("author_name")] List<string>? AuthorName,
    [property: JsonPropertyName("first_publish_year")] int? FirstPublishYear,
    [property: JsonPropertyName("isbn")] List<string>? Isbn,
    [property: JsonPropertyName("cover_i")] int? CoverId);
