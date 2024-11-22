using Domain.Models;
using MediatR;

namespace Application.Queries.BookQueries.GetAll
{
    public record GetAllBooksQuery() : IRequest<List<Book>>;

}
