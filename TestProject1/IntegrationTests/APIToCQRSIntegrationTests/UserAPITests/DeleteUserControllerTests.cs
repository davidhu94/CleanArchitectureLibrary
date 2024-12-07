using API.Controllers;
using Application.Interfaces.ServiceInterfaces;
using Application.Users.UserCommands.DeleteUser;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.UserAPITests
{
    public class DeleteUserControllerTests : IDisposable
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

        [Test]
        public async Task DeleteUser_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            int userId = 1;
            var operationResult = OperationResult<bool>.Success(true, "User deleted successfully.");
            A.CallTo(() => _mediator.Send(A<DeleteUserCommand>.Ignored, default))
                .Returns(operationResult);

            var result = await _controller.DeleteUser(userId);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteUser_ReturnsBadRequest_WhenInvalidId()
        {
            int userId = 0;

            var result = await _controller.DeleteUser(userId);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("User ID must be greater than zero.", badRequestResult.Value);
        }

        [Test]
        public async Task DeleteUser_ReturnsNotFound_WhenUserNotFound()
        {
            int userId = 1;
            var operationResult = OperationResult<bool>.Failure("User with ID 1 not found.");
            A.CallTo(() => _mediator.Send(A<DeleteUserCommand>.Ignored, default))
                .Returns(operationResult);

            var result = await _controller.DeleteUser(userId);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User with ID 1 not found.", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteUser_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            int userId = 1;
            A.CallTo(() => _mediator.Send(A<DeleteUserCommand>.Ignored, default))
                .Throws(new Exception("Database error"));

            var result = await _controller.DeleteUser(userId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while deleting the user.", objectResult.Value);
        }

        [TearDown]
        public void TearDown()
        {
            _mediator = null;
            _logger = null;
            _tokenService = null;
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}
