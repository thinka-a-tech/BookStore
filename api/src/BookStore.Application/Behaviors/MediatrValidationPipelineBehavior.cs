using FluentResults;
using FluentValidation;
using MediatR;

namespace BookStore.Application.Behaviors;

public sealed class MediatrValidationPipelineBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : ResultBase, new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        var response = new TResponse();
        foreach (var failure in failures)
            response.Reasons.Add(new Error(failure.ErrorMessage).WithMetadata("Property", failure.PropertyName));

        return response;
    }
}
