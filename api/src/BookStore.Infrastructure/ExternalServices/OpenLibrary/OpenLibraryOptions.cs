namespace BookStore.Infrastructure.ExternalServices.OpenLibrary;

public sealed class OpenLibraryOptions
{
    public const string SectionName = "OpenLibrary";

    public string BaseUrl { get; set; } = "https://openlibrary.org/";
    public string CoversBaseUrl { get; set; } = "https://covers.openlibrary.org/";
    public string UserAgent { get; set; } = "BookStoreApp/1.0 (contact: dev@example.com)";
    public int TimeoutSeconds { get; set; } = 10;
}
