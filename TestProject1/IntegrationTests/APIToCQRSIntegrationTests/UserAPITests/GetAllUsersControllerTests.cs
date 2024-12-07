using API.Controllers;
using Application.DTOs.UserDTOs;
using Application.Interfaces.ServiceInterfaces;
using Application.Users.UserQueries.GetAll;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.UserAPITests
{
    public class GetAllUsersControllerTests
    {
        private UserController _controller;
        private IMediator _mediator;
        private ILogger<UserController> _logger;
        private ITokenService _tokenService;

        [SetUp]
        public void SetUp()
        {
            _mediator = A.Fake<IMediator>();
            _logger = A.Fake<ILogger<UserController>>();
            _tokenService = A.Fake<ITokenService>();

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
        public async Task GetAllUsers_ReturnsOk_WhenUsersAreFound()
        {
            var users = new List<UserDto>
            {
                new UserDto { Id = 1, UserName = "user1" },
                new UserDto { Id = 2, UserName = "user2" }
            };

            A.CallTo(() => _mediator.Send(A<GetAllUsersQuery>.Ignored, default))
                .Returns(OperationResult<List<UserDto>>.Success(users));

            var result = await _controller.GetAllUsers();

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var returnedUsers = okResult.Value as List<UserDto>;

            Assert.IsNotNull(returnedUsers);
            Assert.AreEqual(2, returnedUsers.Count);
        }

        [Test]
        public async Task GetAllUsers_ReturnsBadRequest_WhenNoUsersFound()
        {
            var operationResult = OperationResult<List<UserDto>>.Failure("No users found.");

            A.CallTo(() => _mediator.Send(A<GetAllUsersQuery>.Ignored, default))
                .Returns(operationResult);

            var result = await _controller.GetAllUsers();

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("No users found.", badRequestResult.Value);
        }

        [Test]
        public async Task GetAllUsers_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            var exceptionMessage = "An error occurred while fetching users.";

            A.CallTo(() => _mediator.Send(A<GetAllUsersQuery>.Ignored, default))
                .Throws(new Exception(exceptionMessage));

            var result = await _controller.GetAllUsers();

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while fetching users.", objectResult.Value);
        }
    }
}
