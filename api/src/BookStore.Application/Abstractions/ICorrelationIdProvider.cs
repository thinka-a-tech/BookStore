namespace BookStore.Application.Abstractions;

public interface ICorrelationIdProvider
{
    string? CorrelationId { get; }
}
