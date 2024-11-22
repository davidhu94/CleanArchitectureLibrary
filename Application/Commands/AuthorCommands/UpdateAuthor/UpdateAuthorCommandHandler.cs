using Infrastructure.Database;
using MediatR;

namespace Application.Commands.AuthorCommands.UpdateAuthor
{
    public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, bool>
    {
        private readonly FakeDatabase _fakeDatabase;

        public UpdateAuthorCommandHandler(FakeDatabase fakeDatabase)
        {
            _fakeDatabase = fakeDatabase ?? throw new ArgumentNullException(nameof(fakeDatabase));
        }

        public Task<bool> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                throw new ArgumentException("Invalid author ID.");
            }

            var authorToUpdate = _fakeDatabase.Authors.FirstOrDefault(author => author.Id == request.Id);

            if (authorToUpdate == null)
            {
                return Task.FromResult(false);
            }

            authorToUpdate.Name = request.Name;

            return Task.FromResult(true);
        }
    }
}
