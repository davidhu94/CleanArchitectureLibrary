using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Authors.AuthorCommands.UpdateAuthor
{
    public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, OperationResult<bool>>
    {
        private readonly IAuthorRepository _repository;
        private readonly ILogger<UpdateAuthorCommandHandler> _logger;

        public UpdateAuthorCommandHandler(IAuthorRepository repository, ILogger<UpdateAuthorCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to update author with ID: {AuthorId}", request.Id);

                if (request.Id <= 0)
                {
                    _logger.LogWarning("Invalid author ID provided: {AuthorId}", request.Id);
                    return OperationResult<bool>.Failure("Invalid author ID.");
                }

                var authorToUpdate = await _repository.GetByIdAsync(request.Id, cancellationToken);

                if (authorToUpdate == null)
                {
                    _logger.LogWarning("Author with ID {AuthorId} not found for update.", request.Id);
                    return OperationResult<bool>.Failure($"Author with ID {request.Id} not found.");
                }

                authorToUpdate.Name = request.Name;

                _logger.LogInformation("Updating author with ID: {AuthorId} to name: {AuthorName}", request.Id, request.Name);

                await _repository.UpdateAsync(authorToUpdate, cancellationToken);

                _logger.LogInformation("Successfully updated author with ID: {AuthorId}.", request.Id);
                return OperationResult<bool>.Success(true, "Author updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating author with ID: {AuthorId}.", request.Id);
                return OperationResult<bool>.Failure("An error occurred while updating the author.", ex.Message);
            }
        }
    }
}
