using Application.DTOs.UserDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;

        public UserController(ITokenService tokenService, IUserRepository userRepository)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            if (string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.UserName))
            {
                return BadRequest("Username and password are required.");
            }

            var existingUser = await _userRepository.GetByUsernameAsync(request.UserName);
            if (existingUser != null)
            {
                return BadRequest("User already exists.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                UserName = request.UserName,
                PasswordHash = passwordHash
            };

            await _userRepository.AddAsync(user);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var userFromDb = await _userRepository.GetByUsernameAsync(request.UserName);

            if (userFromDb == null || !BCrypt.Net.BCrypt.Verify(request.Password, userFromDb.PasswordHash))
            {
                return BadRequest("Invalid credentials");
            }

            string token = _tokenService.CreateToken(userFromDb);
            return Ok(token);
        }
    }
}
