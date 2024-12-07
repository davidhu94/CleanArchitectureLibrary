using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.UserQueries.GetUserByUsername
{
    public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserByUsernameQueryHandler> _logger;

        public GetUserByUsernameQueryHandler(IUserRepository userRepository, ILogger<GetUserByUsernameQueryHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetUserByUsernameQuery for username: {UserName}", request.UserName);

            try
            {
                var user = await _userRepository.GetByUsernameAsync(request.UserName, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("No user found with username: {UserName}", request.UserName);
                }
                else
                {
                    _logger.LogInformation("Successfully found user with username: {UserName}", request.UserName);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user by username: {UserName}", request.UserName);
                throw new ApplicationException($"An error occurred while fetching user with username {request.UserName}.", ex);
            }
        }
    }
}
