using Application.DTOs.UserDTOs;
using Domain.Models;
using MediatR;

namespace Application.Users.UserQueries.GetById
{
    public class GetUserByIdQuery : IRequest<OperationResult<UserDto>>
    {
        public int UserId { get; }

        public GetUserByIdQuery(int userId)
        {
            UserId = userId;
        }
    }
}
