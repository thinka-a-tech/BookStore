using FluentResults;

namespace BookStore.Application.Errors;

public sealed class ConflictError(string message) : Error(message);
