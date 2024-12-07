using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.UserCommands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, OperationResult<bool>>
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(IUserRepository repository, ILogger<UpdateUserCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId <= 0)
            {
                _logger.LogWarning("Invalid UserId received. UserId: {UserId}", request.UserId);
                return OperationResult<bool>.Failure("Invalid user ID.");
            }

            if (string.IsNullOrWhiteSpace(request.UserName) && string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Neither UserName nor Password provided for update. UserId: {UserId}", request.UserId);
                return OperationResult<bool>.Failure("At least one field (UserName or Password) must be updated.");
            }

            try
            {
                _logger.LogInformation("Attempting to fetch user with UserId: {UserId} from the repository.", request.UserId);
                var user = await _repository.GetByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found. UserId: {UserId}", request.UserId);
                    return OperationResult<bool>.Failure($"User with ID {request.UserId} not found.");
                }

                if (!string.IsNullOrWhiteSpace(request.UserName))
                {
                    _logger.LogInformation("Updating UserName for UserId: {UserId} to {UserName}", request.UserId, request.UserName);
                    user.UserName = request.UserName;
                }

                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    _logger.LogInformation("Updating PasswordHash for UserId: {UserId}", request.UserId);
                    user.PasswordHash = hashedPassword;
                }

                await _repository.UpdateAsync(user, cancellationToken);
                _logger.LogInformation("User successfully updated. UserId: {UserId}", request.UserId);

                return OperationResult<bool>.Success(true, "User updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user with UserId: {UserId}", request.UserId);
                return OperationResult<bool>.Failure($"An error occurred while updating user with ID {request.UserId}: {ex.Message}");
            }
        }
    }
}
