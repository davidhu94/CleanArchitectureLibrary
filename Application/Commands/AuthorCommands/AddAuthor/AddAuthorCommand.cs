using Domain.Models;
using MediatR;

namespace Application.Commands.AuthorCommands.AddAuthor
{
    public class AddAuthorCommand : IRequest<int>
    {
        public string Name { get; set; }

        public AddAuthorCommand(string name)
        {
            Name = name;
        }
    }
}
