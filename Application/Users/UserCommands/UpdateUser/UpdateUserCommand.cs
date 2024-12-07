using Domain.Models;
using MediatR;

namespace Application.Users.UserCommands.UpdateUser
{
    public class UpdateUserCommand : IRequest<OperationResult<bool>>
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public UpdateUserCommand(int userId, string userName, string password)
        {
            UserId = userId;
            UserName = userName;
            Password = password;
        }
    }
}
