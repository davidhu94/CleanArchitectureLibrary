using Application.DTOs.BookDTOs;
using Domain.Models;
using MediatR;

namespace Application.Books.BookCommands.AddBook
{
    public class AddBookCommand : IRequest<OperationResult<BookDto>>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }

        public AddBookCommand(string title, string description, int authorId)
        {
            Title = title;
            Description = description;
            AuthorId = authorId;
        }
    }
}
