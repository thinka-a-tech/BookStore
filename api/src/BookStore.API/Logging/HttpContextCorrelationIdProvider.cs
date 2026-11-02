using BookStore.Application.Abstractions;

namespace BookStore.API.Logging;

public sealed class HttpContextCorrelationIdProvider(IHttpContextAccessor accessor) : ICorrelationIdProvider
{
    public string? CorrelationId => accessor.HttpContext?.Items[CorrelationConstants.ItemKey] as string;
}
