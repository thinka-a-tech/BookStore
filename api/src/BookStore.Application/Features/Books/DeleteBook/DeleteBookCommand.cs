using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Books.DeleteBook;

public sealed record DeleteBookCommand(int Id) : IRequest<Result>;
