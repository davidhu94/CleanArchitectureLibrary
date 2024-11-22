using Domain.Models;
using MediatR;

namespace Application.Queries.BookQueries.GetById
{
    public class GetBookByIdQuery : IRequest<Book>
    {
        public int Id { get; }
        public GetBookByIdQuery(int id)
        {
            Id = id;
        }
    }
}
