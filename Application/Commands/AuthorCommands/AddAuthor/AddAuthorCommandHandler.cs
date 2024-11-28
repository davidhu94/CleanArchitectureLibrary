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
                throw new ArgumentException("Author name is required.");
            }

            var newAuthor = new Author(request.Name);
            await _repository.AddAsync(newAuthor);

            return newAuthor.Id;
        }
    }
}
