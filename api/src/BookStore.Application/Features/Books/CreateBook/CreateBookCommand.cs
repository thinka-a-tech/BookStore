using BookStore.Domain.Enums;
using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Books.CreateBook;

public sealed record CreateBookCommand(string Title, string Author, string Isbn, string OpenLibraryKey, Genre Genre,
    decimal Price, int Stock = 0, int? PublishedYear = null, string? CoverUrl = null, string? Description = null)
    : IRequest<Result<int>>;