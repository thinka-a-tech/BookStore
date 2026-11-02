using System.ComponentModel.DataAnnotations;
using BookStore.API.Extensions;
using BookStore.API.Payload.Requests;
using BookStore.Application.Features.Books;
using BookStore.Application.Features.Books.GetBooks;
using BookStore.Application.Features.Books.GetGenreCounts;
using BookStore.Application.Features.Books.UpdateBook;
using BookStore.Domain.Models;
using BookStore.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/books")]
[Produces("application/json")]
public sealed class BooksController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists books with paging and sorting.</summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="sortBy">Field to sort by</param>
    /// <param name="descending">Sort in descending order</param>
    /// <param name="genre">Only return books of this genre</param>
    [HttpGet]
    [ProducesResponseType<PagedResult<BookDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PagedResult<BookDto>>> GetBooks(
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        [FromQuery] BookSortField sortBy = BookSortField.Title,
        [FromQuery] bool descending = false,
        [FromQuery] Genre? genre = null,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetBooksQuery(page, pageSize, sortBy, descending, genre), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToFailureResult(this);
    }

    /// <summary>Counts books per genre; useful for populating the genre filter.</summary>
    [HttpGet("genres")]
    [ProducesResponseType<IReadOnlyList<GenreCount>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<GenreCount>>> GetGenreCounts(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetGenreCountsQuery(), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToFailureResult(this);
    }

    /// <summary>Updates the store-owned fields of a book (genre, price, stock, description) and returns the updated book.</summary>
    /// <param name="id">Book id</param>
    /// <param name="request">New store-owned values</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType<BookDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookDto>> UpdateBook(int id, [FromBody] UpdateBookRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateBookCommand(id, request.Genre, request.Price, request.Stock, request.Description), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToFailureResult(this);
    }
}
