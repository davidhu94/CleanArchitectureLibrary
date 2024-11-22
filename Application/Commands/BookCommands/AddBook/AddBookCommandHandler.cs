using Domain.Models;
using Infrastructure.Database;
using MediatR;

namespace Application.Commands.BookCommands.AddBook
{
    public class AddBookCommandHandler : IRequestHandler<AddBookCommand, int>
    {
        private readonly FakeDatabase _fakeDatabase;

        public AddBookCommandHandler(FakeDatabase fakeDatabase)
        {
            _fakeDatabase = fakeDatabase ?? throw new ArgumentNullException(nameof(fakeDatabase));
        }

        public Task<int> Handle(AddBookCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentException("Title and description are required.");
            }

            var authorExists = _fakeDatabase.Authors.Any(author => author.Id == request.AuthorId);
            if (!authorExists)
            {
                throw new ArgumentException($"Author with ID {request.AuthorId} does not exist.");
            }

            var newBookId = _fakeDatabase.Books.Max(book => book.Id) + 1;

            var newBook = new Book(newBookId, request.Title, request.Description, request.AuthorId);
            _fakeDatabase.Books.Add(newBook);

            return Task.FromResult(newBookId);
        }
    }
}
