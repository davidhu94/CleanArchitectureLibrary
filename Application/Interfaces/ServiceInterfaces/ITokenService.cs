using Domain.Models;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
