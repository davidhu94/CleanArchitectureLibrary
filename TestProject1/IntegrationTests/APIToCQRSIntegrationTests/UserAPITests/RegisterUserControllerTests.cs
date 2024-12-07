using API.Controllers;
using Application.DTOs.UserDTOs;
using Application.Interfaces.ServiceInterfaces;
using Application.Users.UserCommands.AddUser;
using Application.Users.UserQueries.GetUserByUsername;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.UserAPITests
{
    public class RegisterUserControllerTests
    {
        private UserController _controller;
        private IMediator _mediator;
        private ITokenService _tokenService;

        [SetUp]
        public void SetUp()
        {
            _mediator = A.Fake<IMediator>();
            _tokenService = A.Fake<ITokenService>();

            _controller = new UserController(_mediator, _tokenService, A.Fake<ILogger<UserController>>());
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
        public async Task Register_ReturnsBadRequest_WhenUsernameOrPasswordIsMissing()
        {
            var request = new AddUserDto
            {
                UserName = "",
                Password = "ValidPassword123"
            };

            var result = await _controller.Register(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Username and password are required.", badRequestResult.Value);
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenUserAlreadyExists()
        {
            var request = new AddUserDto
            {
                UserName = "existinguser",
                Password = "ValidPassword123"
            };

            A.CallTo(() => _mediator.Send(A<GetUserByUsernameQuery>.That.Matches(q => q.UserName == request.UserName), default))
                .Returns(Task.FromResult(new User()));

            var result = await _controller.Register(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("User already exists.", badRequestResult.Value);
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenCommandFails()
        {
            var request = new AddUserDto
            {
                UserName = "newuser",
                Password = "ValidPassword123"
            };

            A.CallTo(() => _mediator.Send(A<GetUserByUsernameQuery>.Ignored, default))
                .Returns(Task.FromResult<User>(null));

            A.CallTo(() => _mediator.Send(A<AddUserCommand>.Ignored, default))
                .Returns(Task.FromResult(OperationResult<int>.Failure("Failed to add user.")));

            var result = await _controller.Register(request);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Failed to add user.", badRequestResult.Value);
        }

        [Test]
        public async Task Register_ReturnsCreatedAtAction_WhenUserIsSuccessfullyCreated()
        {
            var request = new AddUserDto
            {
                UserName = "newuser",
                Password = "ValidPassword123"
            };

            A.CallTo(() => _mediator.Send(A<GetUserByUsernameQuery>.Ignored, default))
                .Returns(Task.FromResult<User>(null));

            A.CallTo(() => _mediator.Send(A<AddUserCommand>.Ignored, default))
                .Returns(Task.FromResult(OperationResult<int>.Success(1, "User added successfully.")));

            var result = await _controller.Register(request);

            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            Assert.AreEqual("GetUserById", createdResult.ActionName);
            Assert.AreEqual(1, createdResult.RouteValues["id"]);
        }

        [Test]
        public async Task Register_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            var request = new AddUserDto
            {
                UserName = "newuser",
                Password = "ValidPassword123"
            };

            A.CallTo(() => _mediator.Send(A<GetUserByUsernameQuery>.Ignored, default))
                .Throws(new Exception("Unexpected error"));

            var result = await _controller.Register(request);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while registering the user.", objectResult.Value);
        }
    }
}
