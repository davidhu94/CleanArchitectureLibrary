using Domain.Models;
using Infrastructure.Database;
using MediatR;

namespace Application.Queries.AuthorQueries.GetById
{
    public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, Author>
    {
        private readonly FakeDatabase _fakeDatabase;

        public GetAuthorByIdQueryHandler(FakeDatabase fakeDatabase)
        {
            _fakeDatabase = fakeDatabase ?? throw new ArgumentNullException(nameof(fakeDatabase));
        }

        public Task<Author?> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
        {
            var wantedAuthor = _fakeDatabase.Authors.FirstOrDefault(author => author.Id == request.Id);
            return Task.FromResult(wantedAuthor);
        }
    }
}
