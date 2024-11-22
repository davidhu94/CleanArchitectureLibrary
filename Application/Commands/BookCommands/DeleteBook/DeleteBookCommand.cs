using MediatR;

namespace Application.Commands.BookCommands.DeleteBook
{
    public class DeleteBookCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteBookCommand(int id)
        {
            Id = id;
        }
    }
}
