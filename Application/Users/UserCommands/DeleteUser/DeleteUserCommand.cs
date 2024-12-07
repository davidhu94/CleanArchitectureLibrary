using Domain.Models;
using MediatR;

namespace Application.Users.UserCommands.DeleteUser
{
    public class DeleteUserCommand : IRequest<OperationResult<bool>>
    {
        public int UserId { get; set; }

        public DeleteUserCommand(int userId)
        {
            UserId = userId;
        }
    }
}
