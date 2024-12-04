using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IRepository<User>, IUserRepository
    {
        private readonly RealDatabase _db;

        public UserRepository(RealDatabase db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
        public async Task AddAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _db.Users.SingleOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int? id)
        {
            return await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var existingUser = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"No user found with ID {user.Id}");
            }

            //_db.Update(user);
            //await _db.SaveChangesAsync();

            _db.Users.Attach(user); // Attach ensures entity tracking if it's detached.
            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
