using Application.Interfaces.RepositoryInterfaces;
using MediatR;

namespace Application.Commands.BookCommands.DeleteBook
{
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, bool>
    {
        private readonly IBookRepository _repository;

        public DeleteBookCommandHandler(IBookRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                throw new ArgumentException("Invalid book ID.", nameof(request.Id));
            }

            var book = await _repository.GetByIdAsync(request.Id);

            if (book == null)
            {
                return false;
            }

            await _repository.DeleteAsync(request.Id);
            return true;
        }
    }
}
