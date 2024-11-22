using Domain.Models;
using MediatR;

namespace Application.Queries.AuthorQueries.GetAll
{
    public class GetAllAuthorsQuery : IRequest<List<Author>>
    {
    }
}
