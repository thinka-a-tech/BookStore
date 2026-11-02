using Serilog.Context;

namespace BookStore.API.Logging;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationIdHeader = context.Request.Headers[CorrelationConstants.HeaderName].FirstOrDefault();
        var correlationId = !string.IsNullOrWhiteSpace(correlationIdHeader) ? correlationIdHeader! : Guid.NewGuid().ToString("N");

        context.Items[CorrelationConstants.ItemKey] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationConstants.HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
