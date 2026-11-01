using FluentValidation;

namespace BookStore.Application.Features.Books.CreateBook;

public sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Isbn).NotEmpty().MaximumLength(20);
        RuleFor(x => x.OpenLibraryKey).NotEmpty();
        RuleFor(x => x.Genre).IsInEnum();
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PublishedYear).InclusiveBetween(0, DateTime.UtcNow.Year).When(x => x.PublishedYear.HasValue);
        RuleFor(x => x.CoverUrl).MaximumLength(2000).When(x => x.CoverUrl is not null);
    }
}
