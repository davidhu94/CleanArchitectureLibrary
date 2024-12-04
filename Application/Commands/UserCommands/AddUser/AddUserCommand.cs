using MediatR;

namespace Application.Commands.UserCommands.AddUser
{
    public class AddUserCommand : IRequest<int>
    {
        public string UserName { get; }
        public string Password { get; }

        public AddUserCommand(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("UserName cannot be null or empty.", nameof(userName));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            UserName = userName;
            Password = password;
        }
    }
}
