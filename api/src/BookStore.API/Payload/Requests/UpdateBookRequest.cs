using BookStore.Domain.Enums;

namespace BookStore.API.Payload.Requests;

public sealed record UpdateBookRequest(Genre Genre, decimal Price, int Stock = 0, string? Description = null);
