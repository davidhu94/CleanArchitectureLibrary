using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.RepositoryInterfaces
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);

        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int? id);

        Task<bool> DeleteAsync(int id);
    }

    public interface IAuthorRepository : IRepository<Author>
    {
        Task<IEnumerable<Author>> GetAuthorsWithBooksAsync();
    }

    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId);
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        //Task<IEnumerable<User>> GetBooksByUserIdAsync(Guid userId);
    }
}
