using Domain.Models;
using MediatR;

namespace Application.Books.BookCommands.UpdateBook
{
    public class UpdateBookCommand : IRequest<OperationResult<bool>>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AuthorId { get; set; }

        public UpdateBookCommand(int id, string title, string description, int authorId)
        {
            Id = id;
            Title = title;
            Description = description;
            AuthorId = authorId;
        }
    }
}
