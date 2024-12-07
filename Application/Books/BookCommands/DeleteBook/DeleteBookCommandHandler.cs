using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Books.BookCommands.DeleteBook
{
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, OperationResult<bool>>
    {
        private readonly IBookRepository _repository;
        private readonly ILogger<DeleteBookCommandHandler> _logger;

        public DeleteBookCommandHandler(IBookRepository repository, ILogger<DeleteBookCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                _logger.LogWarning("DeleteBookCommand received with invalid ID: {Id}", request.Id);
                return OperationResult<bool>.Failure("Invalid book ID.");
            }

            _logger.LogInformation("Fetching book with ID: {Id} for deletion.", request.Id);
            var book = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (book == null)
            {
                _logger.LogWarning("Book with ID: {Id} not found. Deletion aborted.", request.Id);
                return OperationResult<bool>.Failure($"Book with ID {request.Id} not found.");
            }

            _logger.LogInformation("Deleting book with ID: {Id}.", request.Id);
            await _repository.DeleteAsync(request.Id, cancellationToken);

            _logger.LogInformation("Book with ID: {Id} successfully deleted.", request.Id);
            return OperationResult<bool>.Success(true, "Book deleted successfully.");
        }
    }
}
