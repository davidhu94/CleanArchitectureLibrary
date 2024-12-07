using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class UserRepository : IRepository<User>, IUserRepository
    {
        private readonly RealDatabase _db;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(RealDatabase db, ILogger<UserRepository> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(user);
                _logger.LogInformation($"Attempting to add user: {user.UserName}");

                await _db.Users.AddAsync(user, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Successfully added user: {user.UserName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding user: {UserName}", user.UserName);
                throw new ApplicationException($"An error occurred while adding the user {user.UserName}.", ex);
            }
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Invalid username provided: {Username}", username);
                return null;
            }

            try
            {
                return await _db.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with username: {Username}", username);
                throw new ApplicationException($"An error occurred while fetching the user with username {username}.", ex);
            }

        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _db.Users
                                .AsNoTracking()
                                .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all users.");
                throw new ApplicationException("An error occurred while fetching all users.", ex);
            }
        }

        public async Task<User?> GetByIdAsync(int? id, CancellationToken cancellationToken)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Invalid user ID provided: {UserId}", id);
                return null;
            }

            try
            {
                return await _db.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with ID: {UserId}", id);
                throw new ApplicationException($"An error occurred while fetching the user with ID {id}.", ex);
            }
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(user);

                var existingUser = await _db.Users
                                             .AsNoTracking()
                                             .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

                if (existingUser == null)
                {
                    _logger.LogWarning("Attempted to update non-existing user with ID {UserId}", user.Id);
                    throw new KeyNotFoundException($"No user found with ID {user.Id}");
                }

                _logger.LogInformation($"Updating user: {user.UserName}");
                _db.Users.Attach(user);
                _db.Entry(user).State = EntityState.Modified;
                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Successfully updated user: {user.UserName}");
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "User not found during update.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user: {UserName}", user.UserName);
                throw new ApplicationException($"An error occurred while updating the user {user.UserName}.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _db.Users.FindAsync(new object[] { id }, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("Attempted to delete non-existing user with ID {UserId}", id);
                    return false;
                }

                _logger.LogInformation($"Deleting user with ID {id}");
                _db.Users.Remove(user);
                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Successfully deleted user with ID {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}", id);
                throw new ApplicationException($"An error occurred while deleting the user with ID {id}.", ex);
            }
        }
    }

}

