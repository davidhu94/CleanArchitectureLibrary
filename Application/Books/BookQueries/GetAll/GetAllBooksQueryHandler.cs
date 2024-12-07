using Application.DTOs.BookDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Application.Mappers;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Books.BookQueries.GetAll
{
    public sealed class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, OperationResult<List<BookDto>>>
    {
        private readonly IBookRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<GetAllBooksQueryHandler> _logger;

        private const string BooksCacheKey = "AllBooks";

        public GetAllBooksQueryHandler(IBookRepository repository, IMemoryCache memoryCache, ILogger<GetAllBooksQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<List<BookDto>>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            if (!_memoryCache.TryGetValue(BooksCacheKey, out List<BookDto> cachedBooksDto))
            {
                try
                {
                    _logger.LogInformation("Fetching books from the repository...");

                    var booksFromRepo = await _repository.GetAllAsync(cancellationToken);
                    if (booksFromRepo == null || !booksFromRepo.Any())
                    {
                        _logger.LogWarning("No books found in the repository.");
                        return OperationResult<List<BookDto>>.Failure("No books found.");
                    }

                    var bookDtos = booksFromRepo.Select(BookMapper.ToDto).ToList();
                    _memoryCache.Set(BooksCacheKey, bookDtos, TimeSpan.FromMinutes(10));

                    return OperationResult<List<BookDto>>.Success(bookDtos, "Books fetched successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving books from the repository.");
                    return OperationResult<List<BookDto>>.Failure("An error occurred while fetching books.");
                }
            }

            _logger.LogInformation("Returning books from cache.");
            return OperationResult<List<BookDto>>.Success(cachedBooksDto, "Books fetched from cache.");
        }
    }
}
