using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Queries.AuthorQueries.GetAll
{
    public class GetAllAuthorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, List<Author>>
    {
        private readonly IAuthorRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "allAuthors";

        public GetAllAuthorsQueryHandler(IAuthorRepository repository, IMemoryCache memoryCache)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<List<Author>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
        {
            //var authors = await _repository.GetAllAsync();
            //return authors.ToList();

            if (!_memoryCache.TryGetValue(CacheKey, out List<Author> allAuthors))
            {
                allAuthors = (List<Author>?)await _repository.GetAllAsync() ?? new List<Author>();

                _memoryCache.Set(CacheKey, allAuthors, TimeSpan.FromMinutes(1));
            }

            return allAuthors;

        }
    }
}
