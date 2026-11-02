using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Catalog.SearchCatalog;

public sealed class SearchCatalogHandler(IBookCatalog catalog): IRequestHandler<SearchCatalogQuery, Result<IReadOnlyList<CatalogBookDto>>>
{
    public Task<Result<IReadOnlyList<CatalogBookDto>>> Handle(SearchCatalogQuery query, CancellationToken cancellationToken)
    {
        return catalog.SearchAsync(query.Query.Trim(), query.Limit, cancellationToken);
    }
}
