using Domain.Models;
using MediatR;

namespace Application.Users.UserCommands.AddUser
{
    public class AddUserCommand : IRequest<OperationResult<int>>
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        public AddUserCommand(string userName, string passwordHash)
        {
            UserName = userName;
            PasswordHash = passwordHash;
        }
    }
}
