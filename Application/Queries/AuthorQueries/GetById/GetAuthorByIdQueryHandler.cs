using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;

namespace Application.Queries.AuthorQueries.GetById
{
    public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, Author>
    {
        private readonly IAuthorRepository _repository;

        public GetAuthorByIdQueryHandler(IAuthorRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Author> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
        {
            var wantedAuthor = await _repository.GetByIdAsync(request.Id);

            if (wantedAuthor == null)
            {
                throw new KeyNotFoundException($"Author with ID {request.Id} was not found.");
            }

            return wantedAuthor;
        }
    }
}
