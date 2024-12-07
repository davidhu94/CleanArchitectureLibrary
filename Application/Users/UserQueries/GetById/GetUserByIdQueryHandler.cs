using Application.DTOs.UserDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.UserQueries.GetById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, OperationResult<UserDto>>
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(IUserRepository repository, ILogger<GetUserByIdQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching user with ID {UserId} from the repository.", request.UserId);

            try
            {
                var wantedUser = await _repository.GetByIdAsync(request.UserId, cancellationToken);

                if (wantedUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} was not found.", request.UserId);
                    return OperationResult<UserDto>.Failure($"User with ID {request.UserId} was not found.");
                }

                var userDto = new UserDto
                {
                    Id = wantedUser.Id,
                    UserName = wantedUser.UserName
                };

                _logger.LogInformation("Successfully retrieved user with ID {UserId}.", request.UserId);

                return OperationResult<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user with ID {UserId}.", request.UserId);
                return OperationResult<UserDto>.Failure("An error occurred while fetching the user.");
            }
        }
    }
}
