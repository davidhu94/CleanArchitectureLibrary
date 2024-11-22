using Domain.Models;
using Infrastructure.Database;
using MediatR;

namespace Application.Queries.BookQueries.GetAll
{
    public sealed class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, List<Book>>
    {
        private readonly FakeDatabase _fakeDatabase;

        public GetAllBooksQueryHandler(FakeDatabase fakeDatabase)
        {
            _fakeDatabase = fakeDatabase ?? throw new ArgumentNullException(nameof(fakeDatabase));
        }

        public Task<List<Book>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_fakeDatabase.Books);
        }
    }
}
