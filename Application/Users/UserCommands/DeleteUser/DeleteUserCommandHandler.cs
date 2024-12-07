using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.UserCommands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, OperationResult<bool>>
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(IUserRepository repository, ILogger<DeleteUserCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId <= 0)
            {
                _logger.LogWarning("Delete user failed: Invalid user ID {UserId}.", request.UserId);
                return OperationResult<bool>.Failure("Invalid user ID.");
            }

            try
            {
                _logger.LogInformation("Attempting to delete user with ID {UserId}.", request.UserId);

                var user = await _repository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("Delete user failed: User with ID {UserId} not found.", request.UserId);
                    return OperationResult<bool>.Failure($"User with ID {request.UserId} not found.");
                }

                await _repository.DeleteAsync(user.Id, cancellationToken);
                _logger.LogInformation("Successfully deleted user with ID {UserId}.", request.UserId);

                return OperationResult<bool>.Success(true, "User deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with ID {UserId}.", request.UserId);
                return OperationResult<bool>.Failure($"An error occurred while deleting the user: {ex.Message}");
            }
        }
    }
}
