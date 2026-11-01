using BookStore.Domain.Models;
using BookStore.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Books.GetGenreCounts;

public sealed record GetGenreCountsQuery : IRequest<Result<IReadOnlyList<GenreCount>>>;

public sealed class GetGenreCountsHandler(IBookReadOnlyRepository books): IRequestHandler<GetGenreCountsQuery, Result<IReadOnlyList<GenreCount>>>
{
    public async Task<Result<IReadOnlyList<GenreCount>>> Handle(GetGenreCountsQuery query, CancellationToken cancellationToken)
    {
        return Result.Ok(await books.GetGenreCountsAsync(cancellationToken));
    }
}
