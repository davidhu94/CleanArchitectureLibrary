using API.Controllers;
using Application.DTOs.UserDTOs;
using Application.Interfaces.ServiceInterfaces;
using Application.Users.UserQueries.GetUserByUsername;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.UserAPITests
{
    public class LoginUserControllerTests
    {
        private UserController _controller;
        private IMediator _mediator;
        private ITokenService _tokenService;
        private ILogger<UserController> _logger;

        [SetUp]
        public void SetUp()
        {
            _mediator = A.Fake<IMediator>();
            _tokenService = A.Fake<ITokenService>();
            _logger = A.Fake<ILogger<UserController>>();

            _controller = new UserController(_mediator, _tokenService, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller is IDisposable disposableController)
            {
                disposableController.Dispose();
            }
        }

        [Test]
        public async Task Login_ReturnsBadRequest_WhenUsernameOrPasswordIsMissing()
        {
            var request = new LoginDto
            {
                UserName = "",
                Password = "ValidPassword123"
            };

            var result = await _controller.Login(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Username and password are required.", badRequestResult.Value);
        }

        [Test]
        public async Task Login_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            var request = new LoginDto
            {
                UserName = "nonexistentuser",
                Password = "ValidPassword123"
            };

            A.CallTo(() => _mediator.Send(A<GetUserByUsernameQuery>.That.Matches(q => q.UserName == request.UserName), default))
                .Returns(Task.FromResult<User>(null));

            var result = await _controller.Login(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid credentials.", badRequestResult.Value);
        }

        [Test]
        public async Task Login_ReturnsBadRequest_WhenPasswordIsIncorrect()
        {
            var request = new LoginDto
            {
                UserName = "existinguser",
                Password = "WrongPassword"
            };

            var existingUser = new User
            {
                UserName = "existinguser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123")
            };

            A.CallTo(() => _mediator.Send(A<GetUserByUsernameQuery>.That.Matches(q => q.UserName == request.UserName), default))
                .Returns(Task.FromResult(existingUser));

            var result = await _controller.Login(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid credentials.", badRequestResult.Value);
        }

        [Test]
        public async Task Login_ReturnsOk_WhenCredentialsAreCorrect()
        {
            var request = new LoginDto
            {
                UserName = "existinguser",
                Password = "CorrectPassword123"
            };

            var existingUser = new User
            {
                UserName = "existinguser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123")
            };

            A.CallTo(() => _mediator.Send(A<GetUserByUsernameQuery>.That.Matches(q => q.UserName == request.UserName), default))
                .Returns(Task.FromResult(existingUser));

            A.CallTo(() => _tokenService.CreateToken(existingUser))
                .Returns("GeneratedToken");

            var result = await _controller.Login(request);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;

            var operationResult = okResult.Value as OperationResult<string>;
            Assert.IsNotNull(operationResult);
            Assert.IsTrue(operationResult.IsSuccessful);
            Assert.AreEqual("GeneratedToken", operationResult.Data);
        }


        [Test]
        public async Task Login_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            var request = new LoginDto
            {
                UserName = "existinguser",
                Password = "CorrectPassword123"
            };

            A.CallTo(() => _mediator.Send(A<GetUserByUsernameQuery>.Ignored, default))
                .Throws(new Exception("Unexpected error"));

            var result = await _controller.Login(request);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An error occurred during login.", objectResult.Value);
        }
    }
}
