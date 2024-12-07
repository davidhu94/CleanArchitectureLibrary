using Domain.Models;

namespace Application.Interfaces.RepositoryInterfaces
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity, CancellationToken cancellationToken);
        Task UpdateAsync(T entity, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

        Task<T?> GetByIdAsync(int? id, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }

    public interface IAuthorRepository : IRepository<Author>
    {
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId, CancellationToken cancellationToken);
    }

    public interface IBookRepository : IRepository<Book>
    {
        
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username, CancellationToken cancellation);
        
    }
}
