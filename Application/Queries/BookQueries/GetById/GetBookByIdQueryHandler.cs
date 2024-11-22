using Domain.Models;
using Infrastructure.Database;
using MediatR;

namespace Application.Queries.BookQueries.GetById
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, Book>
    {
        private readonly FakeDatabase _fakeDatabase;

        public GetBookByIdQueryHandler(FakeDatabase fakeDatabase)
        {
            _fakeDatabase = fakeDatabase ?? throw new ArgumentNullException(nameof(fakeDatabase));
        }

        public Task<Book?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var wantedBook = _fakeDatabase.Books.FirstOrDefault(book => book.Id == request.Id);

            return Task.FromResult(wantedBook);
        }
    }

}
