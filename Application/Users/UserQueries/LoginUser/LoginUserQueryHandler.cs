using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Application.Users.UserQueries.LoginUser;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.UserQueries.Login
{
    public sealed class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, OperationResult<string>>
    {
        private readonly IUserRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginUserQueryHandler> _logger;

        public LoginUserQueryHandler(
            IUserRepository repository,
            ITokenService tokenService,
            ILogger<LoginUserQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<string>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to log in user: {UserName}", request.UserName);

            try
            {
                var user = await _repository.GetByUsernameAsync(request.UserName, cancellationToken);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed for username: {UserName}. Invalid credentials.", request.UserName);
                    return OperationResult<string>.Failure("Invalid username or password.");
                }

                var token = _tokenService.CreateToken(user);

                _logger.LogInformation("User {UserName} logged in successfully.", request.UserName);
                return OperationResult<string>.Success(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for username: {UserName}", request.UserName);
                return OperationResult<string>.Failure("An error occurred during login.");
            }
        }
    }
}
