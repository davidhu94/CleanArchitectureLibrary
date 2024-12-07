using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class BookRepository : IRepository<Book>, IBookRepository
    {
        private readonly RealDatabase _db;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(RealDatabase db, ILogger<BookRepository> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task AddAsync(Book book, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(book);
                await _db.Books.AddAsync(book, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new book.");
                throw new ApplicationException("An error occurred while adding the book.", ex);
            }
        }

        public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _db.Books
                                .AsNoTracking()
                                .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all books.");
                throw new ApplicationException("An error occurred while fetching books.", ex);
            }
        }
        public async Task<Book?> GetByIdAsync(int? id, CancellationToken cancellationToken)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Invalid book ID provided: {BookId}", id);
                return null;
            }

            try
            {
                return await _db.Books
                                .AsNoTracking()
                                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching the book with ID: {BookId}", id);
                throw new ApplicationException($"An error occurred while fetching the book with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId, CancellationToken cancellationToken)
        {
            try
            {
                return await _db.Books
                                 .AsNoTracking()
                                 .Where(b => b.AuthorId == authorId)
                                 .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching books for author ID: {AuthorId}", authorId);
                throw new ApplicationException($"An error occurred while fetching books for author ID {authorId}.", ex);
            }
        }

        public async Task UpdateAsync(Book book, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(book);

                var existingBook = await _db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == book.Id, cancellationToken);
                if (existingBook == null)
                {
                    _logger.LogWarning("No book found with ID {BookId} for update.", book.Id);
                    throw new KeyNotFoundException($"No book found with ID {book.Id}");
                }

                _db.Books.Attach(book);
                _db.Entry(book).State = EntityState.Modified;
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Book not found during update.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the book with ID: {BookId}", book.Id);
                throw new ApplicationException("An error occurred while updating the book.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var book = await _db.Books.FindAsync(id, cancellationToken);
                if (book == null)
                {
                    _logger.LogWarning("Book with ID {BookId} not found for deletion.", id);
                    return false;
                }

                _db.Books.Remove(book);
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the book with ID: {BookId}", id);
                throw new ApplicationException("An error occurred while deleting the book.", ex);
            }
        }
    }
}
