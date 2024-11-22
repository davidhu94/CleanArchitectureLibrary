using Domain.Models;
using Infrastructure.Database;
using MediatR;

namespace Application.Queries.AuthorQueries.GetAll
{
    public class GetAllAuthorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, List<Author>>
    {
        private readonly FakeDatabase _fakeDatabase;

        public GetAllAuthorsQueryHandler(FakeDatabase fakeDatabase)
        {
            _fakeDatabase = fakeDatabase ?? throw new ArgumentNullException(nameof(fakeDatabase));
        }

        public Task<List<Author>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_fakeDatabase.Authors);

        }
    }
}
