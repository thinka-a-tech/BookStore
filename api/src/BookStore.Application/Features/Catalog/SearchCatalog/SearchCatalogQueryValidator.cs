using FluentValidation;

namespace BookStore.Application.Features.Catalog.SearchCatalog;

public sealed class SearchCatalogQueryValidator : AbstractValidator<SearchCatalogQuery>
{
    public SearchCatalogQueryValidator()
    {
        RuleFor(x => x.Query).NotEmpty().MinimumLength(2).MaximumLength(200);
        RuleFor(x => x.Limit).InclusiveBetween(1, 20);
    }
}
