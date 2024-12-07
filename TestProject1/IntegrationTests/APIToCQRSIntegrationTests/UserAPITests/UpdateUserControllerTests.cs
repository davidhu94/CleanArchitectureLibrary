using API.Controllers;
using Application.DTOs.UserDTOs;
using Application.Interfaces.ServiceInterfaces;
using Application.Users.UserCommands.UpdateUser;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.UserAPITests
{
    public class UpdateUserControllerTests
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
        public async Task UpdateUser_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            var updateUserDto = new UpdateUserDto { Id = 1, UserName = "updatedUser", PasswordHash = "hashedPassword" };
            var operationResult = OperationResult<bool>.Success(true, "User updated successfully.");
            A.CallTo(() => _mediator.Send(A<UpdateUserCommand>.Ignored, default))
                .Returns(operationResult);

            var result = await _controller.UpdateUser(1, updateUserDto);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdateUser_ReturnsBadRequest_WhenIdMismatchOrNullData()
        {
            var updateUserDto = new UpdateUserDto { Id = 2, UserName = "updatedUser", PasswordHash = "hashedPassword" };

            var result = await _controller.UpdateUser(1, updateUserDto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("The ID in the URL must match the user ID in the body.", badRequestResult.Value);
        }

        [Test]
        public async Task UpdateUser_ReturnsNotFound_WhenUserNotFound()
        {
            var updateUserDto = new UpdateUserDto { Id = 1, UserName = "updatedUser", PasswordHash = "hashedPassword" };
            var operationResult = OperationResult<bool>.Failure("User with ID 1 not found.");
            A.CallTo(() => _mediator.Send(A<UpdateUserCommand>.Ignored, default))
                .Returns(operationResult);

            var result = await _controller.UpdateUser(1, updateUserDto);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User with ID 1 not found.", notFoundResult.Value);
        }

        [Test]
        public async Task UpdateUser_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            var updateUserDto = new UpdateUserDto { Id = 1, UserName = "updatedUser", PasswordHash = "hashedPassword" };
            A.CallTo(() => _mediator.Send(A<UpdateUserCommand>.Ignored, default))
                .Throws(new Exception("Database error"));

            var result = await _controller.UpdateUser(1, updateUserDto);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while updating the user.", objectResult.Value);
        }
    }
}
