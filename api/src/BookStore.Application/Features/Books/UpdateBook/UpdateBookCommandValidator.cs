using FluentValidation;

namespace BookStore.Application.Features.Books.UpdateBook;

public sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Genre).IsInEnum();
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}
