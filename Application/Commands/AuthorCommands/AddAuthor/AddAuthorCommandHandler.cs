using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;

namespace Application.Commands.AuthorCommands.AddAuthor
{
    public class AddAuthorCommandHandler : IRequestHandler<AddAuthorCommand, int>
    {
        private readonly IAuthorRepository _repository;

        public AddAuthorCommandHandler(IAuthorRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<int> Handle(AddAuthorCommand request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentNullException(nameof(request.Name),"Author name is required.");
            }

            var newAuthor = new Author(request.Name);

            try
            {
                await _repository.AddAsync(newAuthor);

                return newAuthor.Id;
            }

            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding the author.", ex);
            }

        }
    }
}
