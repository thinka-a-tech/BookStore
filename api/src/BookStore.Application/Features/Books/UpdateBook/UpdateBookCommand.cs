using BookStore.Domain.Enums;
using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Books.UpdateBook;

public sealed record UpdateBookCommand(int Id, Genre Genre, decimal Price, int Stock = 0, string? Description = null) : IRequest<Result<BookDto>>;