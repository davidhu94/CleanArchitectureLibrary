using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;

namespace Application.Commands.BookCommands.AddBook
{
    public class AddBookCommandHandler : IRequestHandler<AddBookCommand, int>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;

        public AddBookCommandHandler(IBookRepository bookRepository, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
        }

        public async Task<int> Handle(AddBookCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentException("Title and description are required.");
            }

            var authorExists = await _authorRepository.GetByIdAsync(request.AuthorId);
            if (authorExists == null)
            {
                throw new ArgumentException($"Author with ID {request.AuthorId} does not exist.");
            }

            var newBook = new Book
            {
                Title = request.Title,
                Description = request.Description,
                AuthorId = request.AuthorId,
            };

            await _bookRepository.AddAsync(newBook);

            return newBook.Id;
        }
    }
}
