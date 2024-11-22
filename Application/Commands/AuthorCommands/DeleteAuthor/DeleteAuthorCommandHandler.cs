using Infrastructure.Database;
using MediatR;

namespace Application.Commands.AuthorCommands.DeleteAuthor
{
    public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand, bool>
    {
        private readonly FakeDatabase _fakeDatabase;

        public DeleteAuthorCommandHandler(FakeDatabase fakeDatabase)
        {
            _fakeDatabase = fakeDatabase ?? throw new ArgumentNullException(nameof(fakeDatabase));
        }

        public Task<bool> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                throw new ArgumentException("Invalid author ID.");
            }

            var authorToDelete = _fakeDatabase.Authors.FirstOrDefault(author => author.Id == request.Id);

            if (authorToDelete == null)
            {
                return Task.FromResult(false);
            }

            _fakeDatabase.Authors.Remove(authorToDelete);
            return Task.FromResult(true);
        }
    }
}
