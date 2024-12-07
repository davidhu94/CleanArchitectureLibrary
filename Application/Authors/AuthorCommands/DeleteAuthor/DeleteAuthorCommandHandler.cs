using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Authors.AuthorCommands.DeleteAuthor
{
    public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand, OperationResult<bool>>
    {
        private readonly IAuthorRepository _repository;
        private readonly ILogger<DeleteAuthorCommandHandler> _logger;

        public DeleteAuthorCommandHandler(IAuthorRepository repository, ILogger<DeleteAuthorCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to delete author with ID: {AuthorId}", request.Id);

                var author = await _repository.GetByIdAsync(request.Id, cancellationToken);

                if (author == null)
                {
                    _logger.LogWarning("Author with ID {AuthorId} not found for deletion.", request.Id);
                    return OperationResult<bool>.Failure($"Author with ID {request.Id} not found.");
                }


                await _repository.DeleteAsync(request.Id, cancellationToken);

                _logger.LogInformation("Successfully deleted author with ID: {AuthorId}.", request.Id);
                return OperationResult<bool>.Success(true, "Author deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to delete author with ID: {AuthorId}.", request.Id);
                return OperationResult<bool>.Failure("An error occurred while deleting the author.", ex.Message);
            }
        }
    }
}
