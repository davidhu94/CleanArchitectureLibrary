using Application.Interfaces.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Channels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Commands.UserCommands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _repository;

        public UpdateUserCommandHandler(IUserRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId <= 0)
            {
                throw new ArgumentException("Invalid user ID.", nameof(request.UserId));
            }

            if (string.IsNullOrWhiteSpace(request.UserName) && string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("At least one field (UserName or Password) must be updated.");
            }

            try
            {
                var user = await _repository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return false;
                }


                if (!string.IsNullOrWhiteSpace(request.UserName))
                {
                    user.UserName = request.UserName;
                }

                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }

                await _repository.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating user with ID {request.UserId}.", ex);
            }
        }
    }
}
