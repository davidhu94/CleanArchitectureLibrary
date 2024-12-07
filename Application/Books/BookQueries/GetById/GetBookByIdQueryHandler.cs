using Application.DTOs.BookDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Application.Mappers;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Books.BookQueries.GetById
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, OperationResult<BookDto>>
    {
        private readonly IBookRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<GetBookByIdQueryHandler> _logger;

        public GetBookByIdQueryHandler(IBookRepository repository, IMemoryCache cache, ILogger<GetBookByIdQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            if (_cache.TryGetValue(request.Id, out BookDto cachedBook))
            {
                _logger.LogInformation($"Book with ID {request.Id} found in cache.");
                return OperationResult<BookDto>.Success(cachedBook, "Book fetched from cache.");
            }

            _logger.LogInformation($"Book with ID {request.Id} not found in cache. Fetching from database.");
            var wantedBook = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (wantedBook == null)
            {
                _logger.LogWarning($"Book with ID {request.Id} was not found in database.");
                return OperationResult<BookDto>.Failure($"Book with ID {request.Id} was not found.");
            }

            _logger.LogInformation($"Caching book with ID {request.Id} for future requests.");
            var bookDto = BookMapper.ToDto(wantedBook);
            _cache.Set(request.Id, bookDto, TimeSpan.FromMinutes(10));

            return OperationResult<BookDto>.Success(bookDto, "Book fetched successfully.");
        }
    }
}
