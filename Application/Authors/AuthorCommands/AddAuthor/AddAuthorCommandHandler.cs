using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.DTOs.AuthorDTOs;

namespace Application.Authors.AuthorCommands.AddAuthor
{
    public class AddAuthorCommandHandler : IRequestHandler<AddAuthorCommand, OperationResult<AuthorDto>>
    {
        private readonly IAuthorRepository _repository;
        private readonly ILogger<AddAuthorCommandHandler> _logger;

        public AddAuthorCommandHandler(IAuthorRepository repository, ILogger<AddAuthorCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<AuthorDto>> Handle(AddAuthorCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding a new author with name: {AuthorName}", request.Name);

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                _logger.LogWarning("Attempt to add author failed: Author name is required.");
                return OperationResult<AuthorDto>.Failure("Author name is required.");
            }

            var newAuthor = new Author(request.Name);

            try
            {
                await _repository.AddAsync(newAuthor, cancellationToken);

                _logger.LogInformation("Successfully added the new author with ID: {AuthorId}", newAuthor.Id);

                var authorDto = new AuthorDto
                {
                    Id = newAuthor.Id,
                    Name = newAuthor.Name
                };

                return OperationResult<AuthorDto>.Success(authorDto, "Author added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the author: {AuthorName}", request.Name);
                return OperationResult<AuthorDto>.Failure("An error occurred while adding the author.", ex.Message);
            }
        }
    }
}
