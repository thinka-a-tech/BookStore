using BookStore.Application.Abstractions;

namespace BookStore.Infrastructure.ExternalServices;

internal sealed class CorrelationIdDelegatingHandler(ICorrelationIdProvider provider) : DelegatingHandler
{
    private const string HeaderName = "X-Correlation-ID";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (provider.CorrelationId is { } correlationId && !request.Headers.Contains(HeaderName))
        {
            request.Headers.TryAddWithoutValidation(HeaderName, correlationId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
