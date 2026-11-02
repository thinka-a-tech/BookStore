using BookStore.Application.Errors;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Books.UpdateBook;

public sealed class UpdateBookHandler(IRepository<BookEntity, int> repository, IReadOnlyRepository<BookEntity, int> readOnlyRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateBookCommand, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
    {
        var book = await readOnlyRepository.GetByIdAsync(command.Id);
        if (book is null)
        {
            return Result.Fail<BookDto>(new NotFoundError($"Book with id {command.Id} was not found."));
        }

        book.UpdateBook(command.Genre, command.Price, command.Stock, command.Description);

        repository.Update(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(BookDto.From(book));
    }
}
