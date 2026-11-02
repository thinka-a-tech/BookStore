using BookStore.API.Extensions;
using BookStore.Application.Features.Catalog;
using BookStore.Application.Features.Catalog.SearchCatalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/catalog")]
[Produces("application/json")]
public sealed class CatalogController(IMediator mediator) : ControllerBase
{
    /// <summary>Searches Open Library and returns normalized results</summary>
    /// <param name="q">Search term (title, author or ISBN)</param>
    /// <param name="limit">Maximum number of results (1-20)</param>
    [HttpGet("search")]
    [ProducesResponseType<IReadOnlyList<CatalogBookDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<CatalogBookDto>>> Search([FromQuery] string q, [FromQuery] int limit = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new SearchCatalogQuery(q, limit), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToFailureResult(this);
    }
}
