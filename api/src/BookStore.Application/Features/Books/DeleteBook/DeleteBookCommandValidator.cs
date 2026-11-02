using FluentValidation;

namespace BookStore.Application.Features.Books.DeleteBook;

public sealed class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
