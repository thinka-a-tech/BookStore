using BookStore.Application.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToFailureResult(this ResultBase result, ControllerBase controller)
    {
        var detail = string.Join("; ", result.Errors.Select(e => e.Message));

        if (result.HasError<NotFoundError>())
        {
            return controller.Problem(detail: detail, statusCode: StatusCodes.Status404NotFound);
        }

        var validationErrors = result.Errors
            .Where(e => e.Metadata.ContainsKey("Property"))
            .GroupBy(e => e.Metadata["Property"]?.ToString() ?? string.Empty)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());

        return validationErrors.Count > 0
            ? controller.ValidationProblem(new ValidationProblemDetails(validationErrors))
            : controller.Problem(detail: detail);
    }
}
