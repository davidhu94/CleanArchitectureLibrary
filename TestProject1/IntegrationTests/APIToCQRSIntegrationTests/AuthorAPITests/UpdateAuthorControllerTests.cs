using API.Controllers;
using Application.Authors.AuthorCommands.UpdateAuthor;
using Application.DTOs.AuthorDTOs;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.AuthorAPITests
{
    public class UpdateAuthorControllerTests
    {
        private IMediator _mediator;
        private ILogger<AuthorController> _logger;
        private AuthorController _controller;

        [SetUp]
        public void Setup()
        {
            _mediator = A.Fake<IMediator>();
            _logger = A.Fake<ILogger<AuthorController>>();

            _controller = new AuthorController(_mediator, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task UpdateAuthor_ReturnsBadRequest_WhenAuthorDataIsNull()
        {
            var result = await _controller.UpdateAuthor(1, null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Updated author data is required.", badRequestResult.Value);
        }

        [Test]
        public async Task UpdateAuthor_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            var updateAuthorDto = new UpdateAuthorDto { Id = 2, Name = "Updated Author" };
            var result = await _controller.UpdateAuthor(1, updateAuthorDto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("The ID in the URL must match the author ID in the request body.", badRequestResult.Value);
        }

        [Test]
        public async Task UpdateAuthor_ReturnsBadRequest_WhenNameIsEmpty()
        {
            var updateAuthorDto = new UpdateAuthorDto { Id = 1, Name = "" };
            var result = await _controller.UpdateAuthor(1, updateAuthorDto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Author name is required.", badRequestResult.Value);
        }

        [Test]
        public async Task UpdateAuthor_ReturnsNoContent_WhenAuthorIsSuccessfullyUpdated()
        {
            var updateAuthorDto = new UpdateAuthorDto { Id = 1, Name = "Updated Author" };

            var operationResult = OperationResult<bool>.Success(true, "Author updated successfully.");

            A.CallTo(() => _mediator.Send(A<UpdateAuthorCommand>.That.Matches(command => command.Id == updateAuthorDto.Id && command.Name == updateAuthorDto.Name), default))
                .Returns(Task.FromResult(operationResult)); 

            var result = await _controller.UpdateAuthor(1, updateAuthorDto);

            Assert.IsInstanceOf<NoContentResult>(result);
        }


        [Test]
        public async Task UpdateAuthor_ReturnsNotFound_WhenUpdateFails()
        {
            var updateAuthorDto = new UpdateAuthorDto { Id = 1, Name = "Updated Author" };

            var operationResult = OperationResult<bool>.Failure("Author not found");

            A.CallTo(() => _mediator.Send(A<UpdateAuthorCommand>.That.Matches(command => command.Id == updateAuthorDto.Id && command.Name == updateAuthorDto.Name), default))
                .Returns(Task.FromResult(operationResult));

            var result = await _controller.UpdateAuthor(1, updateAuthorDto);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Author not found", notFoundResult.Value);
        }


        [Test]
        public async Task UpdateAuthor_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var updateAuthorDto = new UpdateAuthorDto { Id = 1, Name = "Updated Author" };
            A.CallTo(() => _mediator.Send(A<UpdateAuthorCommand>.That.Matches(command => command.Id == updateAuthorDto.Id && command.Name == updateAuthorDto.Name), default))
                .Throws(new Exception("Unexpected error"));

            var result = await _controller.UpdateAuthor(1, updateAuthorDto);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while updating the author.", objectResult.Value);
        }
    }
}
