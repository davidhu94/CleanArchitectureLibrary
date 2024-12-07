using Domain.Models;
using MediatR;

namespace Application.Users.UserQueries.GetUserByUsername
{
    public class GetUserByUsernameQuery : IRequest<User>
    {
        public string UserName { get; set; }

        public GetUserByUsernameQuery(string userName)
        {
            UserName = userName;
        }
    }
}
