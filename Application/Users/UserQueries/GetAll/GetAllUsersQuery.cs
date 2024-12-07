using Application.DTOs.UserDTOs;
using Domain.Models;
using MediatR;

namespace Application.Users.UserQueries.GetAll
{
    public class GetAllUsersQuery() : IRequest<OperationResult<List<UserDto>>>
    {

    }

}
