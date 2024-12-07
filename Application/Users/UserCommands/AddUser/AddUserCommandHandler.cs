using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.UserCommands.AddUser
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, OperationResult<int>>
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<AddUserCommandHandler> _logger;

        public AddUserCommandHandler(IUserRepository repository, ILogger<AddUserCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<int>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling AddUserCommand for UserName: {UserName}", request.UserName);

            if (string.IsNullOrWhiteSpace(request.UserName))
            {
                _logger.LogWarning("AddUserCommand failed: UserName is empty.");
                return OperationResult<int>.Failure("User Name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.PasswordHash))
            {
                _logger.LogWarning("AddUserCommand failed: Password is empty.");
                return OperationResult<int>.Failure("Password is required.");
            }

            var newUser = new User
            {
                UserName = request.UserName,
                PasswordHash = request.PasswordHash,
            };

            try
            {
                _logger.LogInformation("Adding new user to the repository: {UserName}", request.UserName);
                await _repository.AddAsync(newUser, cancellationToken);

                _logger.LogInformation("Successfully added user with ID: {UserId}", newUser.Id);

                return OperationResult<int>.Success(newUser.Id, "User added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the user with UserName: {UserName}.", request.UserName);
                return OperationResult<int>.Failure($"An error occurred while adding the user: {ex.Message}");
            }
        }
    }
}
