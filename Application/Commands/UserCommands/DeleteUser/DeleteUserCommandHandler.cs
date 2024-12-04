using Application.Interfaces.RepositoryInterfaces;
using MediatR;

namespace Application.Commands.UserCommands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserRepository _repository;

        public DeleteUserCommandHandler(IUserRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId <= 0)
            {
                throw new ArgumentException("Invalid user ID.", nameof(request.UserId));
            }

            try
            {
                // Check if the user exists
                var user = await _repository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    // Optionally log or handle not found
                    return false; // User does not exist, nothing to delete
                }

                // Proceed with deletion
                await _repository.DeleteAsync(user.Id);

                return true; // Indicate successful deletion
            }
            catch (Exception ex)
            {
                // Log or handle exception
                throw new ApplicationException($"An error occurred while deleting user with ID {request.UserId}.", ex);
            }
        }
    }
}
