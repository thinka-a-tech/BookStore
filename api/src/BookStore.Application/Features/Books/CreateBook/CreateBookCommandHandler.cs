using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Books.CreateBook;

public sealed class CreateBookCommandHandler(IRepository<BookEntity, int> repository, IBookReadOnlyRepository bookReadOnlyRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateBookCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        if (await bookReadOnlyRepository.IsbnExistsAsync(command.Isbn, cancellationToken))
        {
            return Result.Fail<int>(new Error($"A book with ISBN '{command.Isbn}' already exists.").WithMetadata("Property", nameof(CreateBookCommand.Isbn)));
        }
            
        var book = new BookEntity(command.Title, command.Author, command.Isbn, command.OpenLibraryKey,
            command.Genre, command.Price, command.Stock, command.PublishedYear, command.CoverUrl, command.Description);

        await repository.AddAsync(book, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(book.Id);
    }
}