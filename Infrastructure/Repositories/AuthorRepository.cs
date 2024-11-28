using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AuthorRepository : IRepository<Author>, IAuthorRepository
    {
        private readonly RealDatabase _db;

        public AuthorRepository(RealDatabase db)
        {
            _db = db;
        }

        public async Task AddAsync(Author author)
        {
            await _db.Authors.AddAsync(author);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _db.Authors.ToListAsync();
        }
        public async Task<Author> GetByIdAsync(int id)
        {
            return await _db.Authors.FindAsync(id);
        }

        public Task<IEnumerable<Author>> GetAuthorsWithBooksAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Author author)
        {
            _db.Authors.Update(author);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var author = await _db.Authors.FindAsync(id);
            if (author == null)
            {
                return false;
            }
            _db.Authors.Remove(author);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
