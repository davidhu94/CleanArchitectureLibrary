using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;

namespace Application.Queries.BookQueries.GetById
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, Book>
    {
        private readonly IBookRepository _repository;

        public GetBookByIdQueryHandler(IBookRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Book?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var wantedBook = await _repository.GetByIdAsync(request.Id);

            if (wantedBook == null)
            {
                throw new KeyNotFoundException($"Book with ID {request.Id} not found.");
            }

            return wantedBook;
        }
    }

}
