using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BookRepository : IRepository<Book>, IBookRepository
    {

        private readonly RealDatabase _db;

        public BookRepository(RealDatabase db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
        public async Task AddAsync(Book book)
        {
            ArgumentNullException.ThrowIfNull(book);
            await _db.Books.AddAsync(book);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _db.Books
                            .AsNoTracking()
                            .ToListAsync();
        }
        public async Task<Book?> GetByIdAsync(int? id)
        {
            return await _db.Books
                            .AsNoTracking()
                            .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId)
        {
            return await _db.Books
                .AsNoTracking()
                .Where(b => b.AuthorId == authorId)
                .ToListAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            ArgumentNullException.ThrowIfNull(book);

            var existingBook = await _db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == book.Id);
            if (existingBook == null)
            {
                throw new KeyNotFoundException($"No book found with ID {book.Id}");
            }

            //_db.Books.Update(book);
            //await _db.SaveChangesAsync();

            _db.Books.Attach(book); // Attach ensures entity tracking if it's detached.
            _db.Entry(book).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
            {
                return false;
            }
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
