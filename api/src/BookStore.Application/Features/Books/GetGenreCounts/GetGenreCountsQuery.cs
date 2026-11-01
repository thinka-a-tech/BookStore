using BookStore.Domain.Models;
using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Books.GetGenreCounts;

public sealed record GetGenreCountsQuery : IRequest<Result<IReadOnlyList<GenreCount>>>;
