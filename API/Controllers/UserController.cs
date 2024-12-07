using Application.DTOs.UserDTOs;
using Application.Interfaces.ServiceInterfaces;
using Application.Users.UserCommands.AddUser;
using Application.Users.UserCommands.DeleteUser;
using Application.Users.UserCommands.UpdateUser;
using Application.Users.UserQueries.GetAll;
using Application.Users.UserQueries.GetById;
using Application.Users.UserQueries.GetUserByUsername;
using Application.Users.UserQueries.LoginUser;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ITokenService tokenService, ILogger<UserController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AddUserDto request)
        {
            if (!IsValidRequest(request))
            {
                return BadRequest("Username and password are required.");
            }
            try
            {
                var existingUser = await CheckIfUserExists(request.UserName);
                if (existingUser != null)
                {
                    return BadRequest("User already exists.");
                }

                var passwordHash = HashPassword(request.Password);
                var result = await RegisterUser(request.UserName, passwordHash);

                if (!result.IsSuccessful)
                {
                    return BadRequest(result.ErrorMessage);
                }

                _logger.LogInformation("Succesfully registered user with username: {UserName}", request.UserName);
                return CreatedAtAction(nameof(GetUserById), new { Id = result.Data }, new { Message = $"User with the User Name: {request.UserName} was created." });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while registering the user.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while registering the user.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Attempt to login with missing username or password.");
                return BadRequest("Username and password are required.");
            }

            try
            {
                var result = await _mediator.Send(new LoginUserQuery(request.UserName, request.Password));


                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Login failed for username: {UserName}. Reason: {Message}", request.UserName, result.ErrorMessage);
                    return BadRequest(result.ErrorMessage);
                }

                _logger.LogInformation("User {UserName} logged in successfully.", request.UserName);
                return Ok(new { Token = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for username: {UserName}", request.UserName);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during login.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var operationResult = await _mediator.Send(new GetAllUsersQuery());

                if (!operationResult.IsSuccessful)
                {
                    _logger.LogWarning("Failed to retrieve users: {ErrorMessage}", operationResult.ErrorMessage);
                    return BadRequest(operationResult.ErrorMessage);
                }

                _logger.LogInformation("Successfully retrieved {UserCount} users.", operationResult.Data.Count);
                return Ok(operationResult.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching users.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID: {UserId}", id);
                return BadRequest("User ID must be greater than zero.");
            }

            try
            {
                var operationResult = await _mediator.Send(new GetUserByIdQuery(id));

                if (!operationResult.IsSuccessful)
                {
                    _logger.LogWarning("Failed to retrieve user with ID {UserId}: {ErrorMessage}", id, operationResult.ErrorMessage);
                    return NotFound(operationResult.ErrorMessage);
                }

                _logger.LogInformation("Successfully retrieved user with ID: {UserId}.", id);
                return Ok(operationResult.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user with ID: {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching the user.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null || id != updateUserDto.Id)
            {
                _logger.LogWarning("Mismatch between URL ID and body ID or null data.");
                return BadRequest("The ID in the URL must match the user ID in the body.");
            }

            try
            {
                var result = await _mediator.Send(new UpdateUserCommand(updateUserDto.Id, updateUserDto.UserName, updateUserDto.PasswordHash));

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Failed to update user with ID {UserId}: {ErrorMessage}", id, result.ErrorMessage);
                    return NotFound(result.ErrorMessage);
                }

                _logger.LogInformation("Successfully updated user with ID: {UserId}.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user with ID {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the user.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID: {UserId}", id);
                return BadRequest("User ID must be greater than zero.");
            }

            try
            {
                var result = await _mediator.Send(new DeleteUserCommand(id));

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Failed to delete user with ID {UserId}: {ErrorMessage}", id, result.ErrorMessage);
                    return NotFound(result.ErrorMessage);
                }

                _logger.LogInformation("Successfully deleted user with ID: {UserId}.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with ID {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the user.");
            }
        }

        private bool IsValidRequest(AddUserDto request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Attempt to register with missing username or password.");
                return false;
            }
            return true;
        }

        private async Task<User?> CheckIfUserExists(string userName)
        {
            return await _mediator.Send(new GetUserByUsernameQuery(userName));
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private async Task<OperationResult<int>> RegisterUser(string userName, string passwordHash)
        {
            var command = new AddUserCommand(userName, passwordHash);
            return await _mediator.Send(command);
        }
    }
}

