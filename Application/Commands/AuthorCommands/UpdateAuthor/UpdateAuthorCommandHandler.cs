using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;

namespace Application.Commands.AuthorCommands.UpdateAuthor
{
    public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, bool>
    {
        private readonly IAuthorRepository _repository;

        public UpdateAuthorCommandHandler(IAuthorRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<bool> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                throw new ArgumentException("Invalid author ID.");
            }

            var authorToUpdate = await _repository.GetByIdAsync(request.Id);

            if (authorToUpdate == null)
            {
                return false;
            }

            authorToUpdate.Name = request.Name;

            await _repository.UpdateAsync(authorToUpdate);

            return true;
        }
    }
}
