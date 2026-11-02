using BookStore.Application.Errors;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace BookStore.Application.Features.Books.DeleteBook;

public sealed class DeleteBookHandler(IRepository<BookEntity, int> repository, IReadOnlyRepository<BookEntity, int> readOnlyRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteBookCommand, Result>
{
    public async Task<Result> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        var book = await readOnlyRepository.GetByIdAsync(command.Id);
        if (book is null)
        {
            return Result.Fail(new NotFoundError($"Book with id {command.Id} was not found."));
        }

        repository.Delete(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
