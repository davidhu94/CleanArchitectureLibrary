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
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task AddAsync(Author author)
        {
            ArgumentNullException.ThrowIfNull(author);
            await _db.Authors.AddAsync(author);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _db.Authors
                            .AsNoTracking()
                            .ToListAsync();
        }
        public async Task<Author?> GetByIdAsync(int? id)
        {
            return await _db.Authors
                            .AsNoTracking()
                            .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Author>> GetAuthorsWithBooksAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Author author)
        {
            ArgumentNullException.ThrowIfNull(author);

            var existingAuthor = await _db.Authors
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync(a => a.Id == author.Id);
            if (existingAuthor == null)
            {
                throw new KeyNotFoundException($"Author with Id {author.Id} not found.");
            }

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
