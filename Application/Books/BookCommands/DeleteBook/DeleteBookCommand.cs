using Domain.Models;
using MediatR;

namespace Application.Books.BookCommands.DeleteBook
{
    public class DeleteBookCommand : IRequest<OperationResult<bool>>
    {
        public int Id { get; set; }

        public DeleteBookCommand(int id)
        {
            Id = id;
        }
    }
}
