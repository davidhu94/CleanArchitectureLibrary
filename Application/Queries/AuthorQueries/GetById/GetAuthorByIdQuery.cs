using Domain.Models;
using MediatR;

namespace Application.Queries.AuthorQueries.GetById
{
    public class GetAuthorByIdQuery : IRequest<Author>
    {
        public int Id { get; }

        public GetAuthorByIdQuery(int id)
        {
            Id = id;
        }
    }
}
