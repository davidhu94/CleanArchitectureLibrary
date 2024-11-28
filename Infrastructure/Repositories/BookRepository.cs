using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BookRepository : IRepository<Book>, IBookRepository
    {

        private readonly RealDatabase _db;

        public BookRepository(RealDatabase db)
        {
            _db = db;
        }
        public async Task AddAsync(Book book)
        {
            await _db.Books.AddAsync(book);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _db.Books.ToListAsync();
        }
        public async Task<Book> GetByIdAsync(int id)
        {
            return await _db.Books.FindAsync(id);
        }

        public Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Book book)
        {
            _db.Books.Update(book);
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
            _db.SaveChanges();
            return true;
        }
    }
}
