using BookStore.Domain.Models;
using BookStore.Domain.Enums;
using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Books.GetBooks;

public sealed record GetBooksQuery(int Page = 1, int PageSize = 10, BookSortField SortBy = BookSortField.Title, bool Descending = false, Genre? Genre = null)
    : IRequest<Result<PagedResult<BookDto>>>;
