using FluentResults;

namespace BookStore.Application.Features.Catalog;

public interface IBookCatalog
{
    Task<Result<IReadOnlyList<CatalogBookDto>>> SearchAsync(string query, int limit = 10, CancellationToken cancellationToken = default);
}
