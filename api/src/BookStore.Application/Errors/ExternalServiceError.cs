using FluentResults;

namespace BookStore.Application.Errors;

public sealed class ExternalServiceError(string message, bool isTimeout = false) : Error(message)
{
    public bool IsTimeout { get; } = isTimeout;
}
