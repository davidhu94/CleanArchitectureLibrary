using Infrastructure.Database;
using MediatR;

namespace Application.Commands.BookCommands.UpdateBook
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, bool>
    {
        private readonly FakeDatabase _fakeDatabase;

        public UpdateBookCommandHandler(FakeDatabase fakeDatabase)
        {
            _fakeDatabase = fakeDatabase ?? throw new ArgumentNullException(nameof(fakeDatabase));
        }
        public Task<bool> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                throw new ArgumentException("Invalid book ID.");
            }

            var bookToUpdate = _fakeDatabase.Books.FirstOrDefault(book => book.Id == request.Id);

            if (bookToUpdate == null)
            {
                return Task.FromResult(false);
            }

            bookToUpdate.Title = request.Title;
            bookToUpdate.Description = request.Description;
            bookToUpdate.AuthorId = request.AuthorId;

            return Task.FromResult(true);
        }
    }
}
