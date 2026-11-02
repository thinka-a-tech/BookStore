using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Catalog.SearchCatalog;

public sealed record SearchCatalogQuery(string Query, int Limit = 10) : IRequest<Result<IReadOnlyList<CatalogBookDto>>>;
