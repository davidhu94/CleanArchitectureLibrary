using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;

namespace Application.Commands.UserCommands.AddUser
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, int>
    {
        private readonly IUserRepository _repository;

        public AddUserCommandHandler(IUserRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<int> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
                throw new ArgumentException("User Name is required.", nameof(request.UserName));

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.", nameof(request.Password));

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                UserName = request.UserName,
                PasswordHash = hashedPassword,
            };

            try
            {

                await _repository.AddAsync(newUser);

                return newUser.Id;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding the user.", ex);
            }

        }
    }
}
