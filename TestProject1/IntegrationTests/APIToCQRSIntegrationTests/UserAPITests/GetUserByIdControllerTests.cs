using API.Controllers;
using Application.DTOs.UserDTOs;
using Application.Interfaces.ServiceInterfaces;
using Application.Users.UserQueries.GetById;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.UserAPITests
{
    public class GetUserByIdControllerTests
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
        public async Task GetUserById_ReturnsOk_WhenUserFound()
        {
            var user = new UserDto { Id = 1, UserName = "user1" };
            var operationResult = OperationResult<UserDto>.Success(user);

            A.CallTo(() => _mediator.Send(A<GetUserByIdQuery>.Ignored, default))
                .Returns(operationResult);

            var result = await _controller.GetUserById(1);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var returnedUser = okResult.Value as UserDto;

            Assert.IsNotNull(returnedUser);
            Assert.AreEqual(1, returnedUser.Id);
            Assert.AreEqual("user1", returnedUser.UserName);
        }

        [Test]
        public async Task GetUserById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            var result = await _controller.GetUserById(0);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("User ID must be greater than zero.", badRequestResult.Value);
        }

        [Test]
        public async Task GetUserById_ReturnsNotFound_WhenUserNotFound()
        {
            var operationResult = OperationResult<UserDto>.Failure("User with ID 999 was not found.");
            A.CallTo(() => _mediator.Send(A<GetUserByIdQuery>.Ignored, default))
                .Returns(operationResult);

            var result = await _controller.GetUserById(999);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User with ID 999 was not found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetUserById_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            A.CallTo(() => _mediator.Send(A<GetUserByIdQuery>.Ignored, default))
                .Throws(new Exception("Database error"));

            var result = await _controller.GetUserById(1);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while fetching the user.", objectResult.Value);
        }
    }
}
