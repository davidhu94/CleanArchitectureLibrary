using Domain.Models;
using MediatR;

namespace Application.Queries.UserQueries.GetAll
{
    public record GetAllUsersQuery() : IRequest<List<User>>;

}
