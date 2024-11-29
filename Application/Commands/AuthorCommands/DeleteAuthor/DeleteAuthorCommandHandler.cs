using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;

namespace Application.Commands.AuthorCommands.DeleteAuthor
{
    public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand, bool>
    {
        private readonly IAuthorRepository _repository;

        public DeleteAuthorCommandHandler(IAuthorRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<bool> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
        {
            var author = await _repository.GetByIdAsync(request.Id);

            if (author == null)
            {
                return false;
            }

            await _repository.DeleteAsync(request.Id);
            return true;
        }
    }
}
