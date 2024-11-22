using Infrastructure.Database;
using MediatR;

namespace Application.Commands.BookCommands.DeleteBook
{
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, bool>
    {
        private readonly FakeDatabase _fakeDatabase;

        public DeleteBookCommandHandler(FakeDatabase fakeDatabase)
        {
            _fakeDatabase = fakeDatabase ?? throw new ArgumentNullException(nameof(fakeDatabase));
        }

        public Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                throw new ArgumentException("Invalid author ID.");
            }

            var bookToDelete = _fakeDatabase.Books.FirstOrDefault(book => book.Id == request.Id);

            if (bookToDelete == null)
            {
                return Task.FromResult(false);
            }

            _fakeDatabase.Books.Remove(bookToDelete);
            return Task.FromResult(true);
        }
    }
}
