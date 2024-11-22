using MediatR;

namespace Application.Commands.AuthorCommands.UpdateAuthor
{
    public class UpdateAuthorCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public UpdateAuthorCommand(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
