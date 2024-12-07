using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Books.BookCommands.UpdateBook
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, OperationResult<bool>>
    {
        private readonly IBookRepository _repository;
        private readonly ILogger<UpdateBookCommandHandler> _logger;

        public UpdateBookCommandHandler(IBookRepository repository, ILogger<UpdateBookCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                _logger.LogError("Invalid Book ID: {BookId}", request.Id);
                return OperationResult<bool>.Failure("Book ID must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogError("Book title is required. Book ID: {BookId}", request.Id);
                return OperationResult<bool>.Failure("Book title is required.");
            }

            if (request.AuthorId <= 0)
            {
                _logger.LogError("Invalid Author ID: {AuthorId} for Book ID: {BookId}", request.AuthorId, request.Id);
                return OperationResult<bool>.Failure("A valid author ID is required.");
            }

            var bookToUpdate = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (bookToUpdate == null)
            {
                _logger.LogWarning("Book with ID {BookId} not found.", request.Id);
                return OperationResult<bool>.Failure($"Book with ID {request.Id} not found.");
            }

            _logger.LogInformation("Updating Book with ID {BookId}", request.Id);

            bookToUpdate.Title = request.Title;
            bookToUpdate.Description = request.Description;
            bookToUpdate.AuthorId = request.AuthorId;

            await _repository.UpdateAsync(bookToUpdate, cancellationToken);

            _logger.LogInformation("Successfully updated Book with ID {BookId}", request.Id);

            return OperationResult<bool>.Success(true, "Book updated successfully.");
        }
    }
}
