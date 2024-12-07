using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.UserQueries.LoginUser
{
    public class LoginUserQuery : IRequest<OperationResult<string>>
    {
        public string UserName { get; }
        public string Password { get; }

        public LoginUserQuery(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
