using Application.DTOs.BookDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Books.BookCommands.AddBook
{
    public class AddBookCommandHandler : IRequestHandler<AddBookCommand, OperationResult<BookDto>>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ILogger<AddBookCommandHandler> _logger;

        public AddBookCommandHandler(
            IBookRepository bookRepository,
            IAuthorRepository authorRepository,
            ILogger<AddBookCommandHandler> logger)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<BookDto>> Handle(AddBookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling AddBookCommand for Title: {Title}, AuthorId: {AuthorId}", request.Title, request.AuthorId);

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogWarning("AddBookCommand failed: Title is empty.");
                return OperationResult<BookDto>.Failure("Title is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                _logger.LogWarning("AddBookCommand failed: Description is empty.");
                return OperationResult<BookDto>.Failure("Description is required.");
            }

            var authorExists = await _authorRepository.GetByIdAsync(request.AuthorId, cancellationToken);
            if (authorExists == null)
            {
                _logger.LogWarning("AddBookCommand failed: Author with ID {AuthorId} does not exist.", request.AuthorId);
                return OperationResult<BookDto>.Failure($"Author with ID {request.AuthorId} does not exist.");
            }

            var newBook = new Book
            {
                Title = request.Title,
                Description = request.Description,
                AuthorId = request.AuthorId,
            };

            _logger.LogInformation("Adding new book to the repository: {Title}", request.Title);
            await _bookRepository.AddAsync(newBook, cancellationToken);

            var newBookDto = new BookDto
            {
                Id = newBook.Id,
                Title = newBook.Title,
                Description = newBook.Description,
                AuthorId = newBook.AuthorId
            };

            _logger.LogInformation("Successfully added book with ID: {BookId}", newBook.Id);

            return OperationResult<BookDto>.Success(newBookDto, "Book added successfully.");
        }
    }
}
