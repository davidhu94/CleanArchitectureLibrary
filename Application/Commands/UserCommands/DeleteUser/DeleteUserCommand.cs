using MediatR;

namespace Application.Commands.UserCommands.DeleteUser
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public int UserId { get; set; }

        public DeleteUserCommand(int userId)
        {
            UserId = userId;
        }
    }
}
