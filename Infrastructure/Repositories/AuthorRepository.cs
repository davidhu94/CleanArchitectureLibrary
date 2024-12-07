using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class AuthorRepository : IRepository<Author>, IAuthorRepository
    {
        private readonly RealDatabase _db;
        private readonly ILogger<AuthorRepository> _logger;

        public AuthorRepository(RealDatabase db, ILogger<AuthorRepository> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAsync(Author author, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(author);
                await _db.Authors.AddAsync(author, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new author.");
                throw new ApplicationException("An error occurred while adding the author.", ex);
            }
        }

        public async Task<IEnumerable<Author>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _db.Authors
                                .AsNoTracking()
                                .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all authors.");
                throw new ApplicationException("An error occurred while fetching authors.", ex);
            }
        }

        public async Task<Author?> GetByIdAsync(int? id, CancellationToken cancellationToken)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Invalid author ID provided: {AuthorId}", id);
                return null;
            }

            try
            {
                return await _db.Authors
                                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching the author with ID: {AuthorId}", id);
                throw new ApplicationException($"An error occurred while fetching the author with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<Author>> GetAuthorsWithBooksAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Author author, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(author);

                var existingAuthor = await _db.Authors
                                               .FirstOrDefaultAsync(a => a.Id == author.Id, cancellationToken);
                if (existingAuthor == null)
                {
                    _logger.LogWarning("Author with ID {AuthorId} not found.", author.Id);
                    throw new KeyNotFoundException($"Author with ID {author.Id} not found.");
                }

                _db.Authors.Update(author);
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Author not found during update.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the author with ID: {AuthorId}", author.Id);
                throw new ApplicationException("An error occurred while updating the author.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var author = await _db.Authors.FindAsync(new object[] { id }, cancellationToken);
                if (author == null)
                {
                    _logger.LogWarning("Author with ID {AuthorId} not found for deletion.", id);
                    return false;
                }

                _db.Authors.Remove(author);
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the author with ID: {AuthorId}", id);
                throw new ApplicationException("An error occurred while deleting the author.", ex);
            }
        }

        public Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
