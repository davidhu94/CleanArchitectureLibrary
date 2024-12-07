using Application.DTOs.AuthorDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Authors.AuthorQueries.GetById
{
    public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, OperationResult<AuthorDto>>
    {
        private readonly IAuthorRepository _repository;
        private readonly ILogger<GetAuthorByIdQueryHandler> _logger;

        public GetAuthorByIdQueryHandler(IAuthorRepository repository, ILogger<GetAuthorByIdQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<AuthorDto>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching author with ID: {AuthorId}", request.Id);

            var wantedAuthor = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (wantedAuthor == null)
            {
                _logger.LogWarning("Author with ID {AuthorId} was not found.", request.Id);
                return OperationResult<AuthorDto>.Failure($"Author with ID {request.Id} was not found.", "Author retrieval failed");
            }

            _logger.LogInformation("Author with ID {AuthorId} retrieved successfully.", request.Id);

            var authorDto = new AuthorDto
            {
                Id = wantedAuthor.Id,
                Name = wantedAuthor.Name
            };

            return OperationResult<AuthorDto>.Success(authorDto, "Author retrieved successfully.");
        }
    }
}
