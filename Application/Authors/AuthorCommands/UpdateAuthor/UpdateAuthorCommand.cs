using Domain.Models;
using MediatR;

namespace Application.Authors.AuthorCommands.UpdateAuthor
{
    public class UpdateAuthorCommand : IRequest<OperationResult<bool>>
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
