
using Application.Interfaces.RepositoryInterfaces;
using MediatR;

namespace Application.Commands.BookCommands.UpdateBook
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, bool>
    {
        private readonly IBookRepository _repository;

        public UpdateBookCommandHandler(IBookRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<bool> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                throw new ArgumentException("Invalid book ID.");
            }

            var bookToUpdate = await _repository.GetByIdAsync(request.Id);

            if (bookToUpdate == null)
            {
                return false;
            }

            bookToUpdate.Title = request.Title;
            bookToUpdate.Description = request.Description;
            bookToUpdate.AuthorId = request.AuthorId;

            await _repository.UpdateAsync(bookToUpdate);
            return true;
        }
    }
}
