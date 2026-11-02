using FluentResults;

namespace BookStore.Application.Errors;

public sealed class NotFoundError(string message) : Error(message);
