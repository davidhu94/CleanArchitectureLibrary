using API.Controllers;
using Application.Authors.AuthorCommands.DeleteAuthor;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.AuthorAPITests
{
    public class DeleteAuthorControllerTests
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
        public async Task DeleteAuthor_ReturnsBadRequest_WhenIdIsInvalid()
        {
            int invalidId = -1;

            var result = await _controller.DeleteAuthor(invalidId);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid author ID.", badRequestResult.Value);
        }

        [Test]
        public async Task DeleteAuthor_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            int validId = 1;
            var operationResult = OperationResult<bool>.Failure("Author not found");

            A.CallTo(() => _mediator.Send(A<DeleteAuthorCommand>.That.Matches(command => command.Id == validId), default))
                .Returns(Task.FromResult(operationResult));

            var result = await _controller.DeleteAuthor(validId);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Author not found", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteAuthor_ReturnsNoContent_WhenAuthorIsSuccessfullyDeleted()
        {
            int validId = 1;
            var operationResult = OperationResult<bool>.Success(true, "Author deleted successfully");

            A.CallTo(() => _mediator.Send(A<DeleteAuthorCommand>.That.Matches(command => command.Id == validId), default))
                .Returns(Task.FromResult(operationResult));

            var result = await _controller.DeleteAuthor(validId);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteAuthor_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            int validId = 1;
            A.CallTo(() => _mediator.Send(A<DeleteAuthorCommand>.That.Matches(command => command.Id == validId), default))
                .Throws(new Exception("Something went wrong"));

            var result = await _controller.DeleteAuthor(validId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while deleting the author.", objectResult.Value);
        }
    }
}
