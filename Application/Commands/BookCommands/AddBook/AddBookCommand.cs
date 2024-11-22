using MediatR;

namespace Application.Commands.BookCommands.AddBook
{
    public class AddBookCommand : IRequest<int>
    {
        //public Book NewBook { get; }
        //public AddBookCommand(Book newBook)
        //{
        //    NewBook = newBook;
        //}

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
