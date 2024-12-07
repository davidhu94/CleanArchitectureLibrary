using Application.DTOs.AuthorDTOs;
using Domain.Models;
using MediatR;

namespace Application.Authors.AuthorCommands.AddAuthor
{
    public class AddAuthorCommand : IRequest<OperationResult<AuthorDto>>
    {
        public string Name { get; set; }

        public AddAuthorCommand(string name)
        {
            Name = name;
        }
    }
}
