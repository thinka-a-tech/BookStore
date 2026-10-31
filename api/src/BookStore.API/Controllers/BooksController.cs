using BookStore.Application.Features.Books;
using BookStore.Application.Features.Books.GetBooks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/books")]
[Produces("application/json")]
public sealed class BooksController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists all books</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<BookDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<BookDto>>> GetBooks(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetBooksQuery(), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(detail: string.Join("; ", result.Errors.Select(e => e.Message)));
    }
}
