using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Books.GetBooks;

public sealed record GetBooksQuery : IRequest<Result<IReadOnlyList<BookDto>>>;

public sealed class GetBooksHandler(IReadOnlyRepository<BookEntity, int> books)
    : IRequestHandler<GetBooksQuery, Result<IReadOnlyList<BookDto>>>
{
    public async Task<Result<IReadOnlyList<BookDto>>> Handle(GetBooksQuery query, CancellationToken cancellationToken)
    {
        var entities = await books.GetAllAsync(cancellationToken);
        return Result.Ok<IReadOnlyList<BookDto>>(entities.Select(BookDto.From).ToList());
    }
}
