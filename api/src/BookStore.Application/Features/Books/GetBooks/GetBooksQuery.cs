using System.Linq.Expressions;
using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Books.GetBooks;

public sealed record GetBooksQuery(int Page = 1, int PageSize = 10, BookSortField SortBy = BookSortField.Title, bool Descending = false)
    : IRequest<Result<PagedResult<BookDto>>>;

public sealed class GetBooksHandler(IReadOnlyRepository<BookEntity, int> books) : IRequestHandler<GetBooksQuery, Result<PagedResult<BookDto>>>
{
    public async Task<Result<PagedResult<BookDto>>> Handle(GetBooksQuery query, CancellationToken cancellationToken)
    {
        var sortExpression = SortExpression(query.SortBy);
        var paged = await books.GetPagedAsync(
            query.Page, query.PageSize, sortExpression, query.Descending, cancellationToken);

        var items = paged.Items.Select(BookDto.From).ToList();
        return Result.Ok(new PagedResult<BookDto>(items, paged.Page, paged.PageSize, paged.TotalCount));
    }

    private static Expression<Func<BookEntity, object?>> SortExpression(BookSortField sortField) => sortField switch
    {
        BookSortField.Author => b => b.Author,
        BookSortField.Genre => b => b.Genre,
        BookSortField.Price => b => b.Price,
        BookSortField.PublishedYear => b => b.PublishedYear,
        BookSortField.Stock => b => b.Stock,
        _ => b => b.Title
    };
}
