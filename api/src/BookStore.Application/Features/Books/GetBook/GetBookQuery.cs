using BookStore.Application.Errors;
using FluentResults;
using FluentValidation;
using MediatR;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Application.Features.Books.GetBook;

public sealed record GetBookQuery(int Id) : IRequest<Result<BookDto>>;

public sealed class GetBookQueryValidator : AbstractValidator<GetBookQuery>
{
    public GetBookQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public sealed class GetBookQueryHandler(IReadOnlyRepository<BookEntity, int> books) : IRequestHandler<GetBookQuery, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(GetBookQuery query, CancellationToken cancellationToken)
    {
        var book = await books.GetByIdAsync(query.Id);

        return book is null
            ? Result.Fail<BookDto>(new NotFoundError($"Book with id {query.Id} was not found."))
            : Result.Ok(BookDto.From(book));
    }
}
