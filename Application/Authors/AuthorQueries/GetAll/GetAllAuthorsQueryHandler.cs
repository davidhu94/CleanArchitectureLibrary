using Application.DTOs.AuthorDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Authors.AuthorQueries.GetAll
{
    public class GetAllAuthorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, OperationResult<List<AuthorDto>>>
    {
        private readonly IAuthorRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<GetAllAuthorsQueryHandler> _logger;
        private const string CacheKey = "allAuthors";

        public GetAllAuthorsQueryHandler(IAuthorRepository repository, IMemoryCache memoryCache, ILogger<GetAllAuthorsQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<List<AuthorDto>>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all authors from the database or cache.");

            List<AuthorDto> authorDtos = null;

            try
            {
                if (!_memoryCache.TryGetValue(CacheKey, out List<Author> allAuthors))
                {
                    _logger.LogInformation("Authors not found in cache, fetching from the repository.");

                    allAuthors = (List<Author>?)await _repository.GetAllAsync(cancellationToken) ?? new List<Author>();

                    _memoryCache.Set(CacheKey, allAuthors, TimeSpan.FromMinutes(1));

                    _logger.LogInformation("Authors fetched from the repository and cached.");
                }
                else
                {
                    _logger.LogInformation("Authors found in cache.");
                }

                authorDtos = allAuthors.Select(author => new AuthorDto
                {
                    Id = author.Id,
                    Name = author.Name
                }).ToList();

                _logger.LogInformation("Successfully mapped {AuthorCount} authors to DTOs.", authorDtos.Count);
                return OperationResult<List<AuthorDto>>.Success(authorDtos, "Authors retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching authors.");
                return OperationResult<List<AuthorDto>>.Failure("An error occurred while fetching authors.", ex.Message);
            }
        }
    }
}
