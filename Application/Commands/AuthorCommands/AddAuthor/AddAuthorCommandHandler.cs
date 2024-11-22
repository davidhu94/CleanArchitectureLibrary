using Domain.Models;
using Infrastructure.Database;
using MediatR;

namespace Application.Commands.AuthorCommands.AddAuthor
{
    public class AddAuthorCommandHandler : IRequestHandler<AddAuthorCommand, int>
    {
        private readonly FakeDatabase _fakeDatabase;

        public AddAuthorCommandHandler(FakeDatabase fakeDatabase)
        {
            _fakeDatabase = fakeDatabase ?? throw new ArgumentNullException(nameof(fakeDatabase));
        }
        public Task<int> Handle(AddAuthorCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Author name is required.");
            }

            var newAuthorId = _fakeDatabase.Authors.Max(author => author.Id) + 1;

            var newAuthor = new Author(newAuthorId, request.Name);

            _fakeDatabase.Authors.Add(newAuthor);

            return Task.FromResult(newAuthorId);
        }
    }
}
