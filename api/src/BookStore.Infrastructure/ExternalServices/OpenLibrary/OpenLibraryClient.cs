using System.Net.Http.Json;
using System.Text.Json;
using BookStore.Application.Errors;
using BookStore.Application.Features.Catalog;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookStore.Infrastructure.ExternalServices.OpenLibrary;

internal sealed class OpenLibraryClient(HttpClient httpClient, IOptions<OpenLibraryOptions> options, ILogger<OpenLibraryClient> logger) : IBookCatalog
{
    private const string Fields = "key,title,author_name,first_publish_year,isbn,cover_i";

    private readonly OpenLibraryOptions _options = options.Value;

    public async Task<Result<IReadOnlyList<CatalogBookDto>>> SearchAsync(string query, int limit = 10, CancellationToken cancellationToken = default)
    {
        var url = $"search.json?q={Uri.EscapeDataString(query)}&fields={Fields}&limit={limit}";

        try
        {
            logger.LogInformation("Searching Open Library for {Query} with limit {Limit}", query, limit);

            using var response = await httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Open Library returned status {StatusCode} for query {Query}", (int)response.StatusCode, query);
                return Result.Fail(new ExternalServiceError($"Open Library returned status {(int)response.StatusCode}."));
            }

            var payload = await response.Content.ReadFromJsonAsync<OpenLibrarySearchResponse>(cancellationToken);
            var books = (payload?.Docs ?? [])
                .Select(Map)
                .OfType<CatalogBookDto>()
                .ToList();

            logger.LogInformation("Open Library returned {ReturnedCount} of {FoundCount} results for {Query}", books.Count, payload?.NumFound ?? 0, query);

            return Result.Ok<IReadOnlyList<CatalogBookDto>>(books);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("Open Library request timed out for query {Query}", query);
            return Result.Fail(new ExternalServiceError("Open Library did not respond in time.", isTimeout: true));
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Open Library request failed for query {Query}", query);
            return Result.Fail(new ExternalServiceError("Open Library is currently unavailable."));
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Open Library returned an unreadable response for query {Query}", query);
            return Result.Fail(new ExternalServiceError("Open Library returned an unexpected response."));
        }
    }

    // Results without a key, title or ISBN cannot be added to the store, so they are dropped.
    private CatalogBookDto? Map(OpenLibraryDoc doc)
    {
        var isbn = doc.Isbn?.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i));
        if (string.IsNullOrWhiteSpace(doc.Key) || string.IsNullOrWhiteSpace(doc.Title) || isbn is null)
            return null;

        var authors = doc.AuthorName?.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
        var author = authors is { Count: > 0 } ? string.Join(", ", authors) : "Unknown";

        var coversBase = _options.CoversBaseUrl.TrimEnd('/');
        var coverUrl = doc.CoverId is { } coverId
            ? $"{coversBase}/b/id/{coverId}-M.jpg"
            : $"{coversBase}/b/isbn/{isbn}-M.jpg";

        return new CatalogBookDto(doc.Title, author, isbn, doc.FirstPublishYear, coverUrl, doc.Key);
    }
}
