using BookStore.Domain.Enums;

namespace BookStore.Domain.Models;

public sealed record GenreCount(Genre Genre, int Count);
