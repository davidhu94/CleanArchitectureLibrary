using Application.DTOs.UserDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.UserQueries.GetAll
{
    public sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, OperationResult<List<UserDto>>>
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(IUserRepository repository, ILogger<GetAllUsersQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all users from the repository.");

            try
            {
                var users = await _repository.GetAllAsync(cancellationToken);

                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No users found in the repository.");
                    return OperationResult<List<UserDto>>.Failure("No users found.");
                }

                var userDtos = users.Select(user => new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName
                }).ToList();

                _logger.LogInformation("Successfully retrieved {UserCount} users from the repository.", userDtos.Count);
                return OperationResult<List<UserDto>>.Success(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users from the repository.");
                return OperationResult<List<UserDto>>.Failure("An error occurred while fetching users.");
            }
        }
    }
}
