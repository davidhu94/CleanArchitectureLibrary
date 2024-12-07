using API.Controllers;
using Application.Authors.AuthorCommands.AddAuthor;
using Application.DTOs.AuthorDTOs;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.AuthorAPITests
{
    public class AddAuthorControllerTests
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
        public async Task AddAuthor_ReturnsCreatedAtAction_WhenAuthorIsSuccessfullyAdded()
        {
            var addAuthorDto = new AddAuthorDto { Name = "New Author" };
            var resultData = new AuthorDto { Id = 1, Name = "New Author" };
            var operationResult = OperationResult<AuthorDto>.Success(resultData);

            A.CallTo(() => _mediator.Send(A<AddAuthorCommand>.That.Matches(command => command.Name == addAuthorDto.Name), default))
                .Returns(Task.FromResult(operationResult));

            var result = await _controller.AddAuthor(addAuthorDto);

            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            var returnedAuthor = createdResult.Value as AuthorDto;
            Assert.AreEqual(1, returnedAuthor.Id);
            Assert.AreEqual("New Author", returnedAuthor.Name);
        }

        [Test]
        public async Task AddAuthor_ReturnsBadRequest_WhenAuthorDataIsNull()
        {
            var result = await _controller.AddAuthor(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Author data is required.", badRequestResult.Value);
        }

        [Test]
        public async Task AddAuthor_ReturnsBadRequest_WhenAddAuthorFails()
        {
            var addAuthorDto = new AddAuthorDto { Name = "New Author" };
            var operationResult = OperationResult<AuthorDto>.Failure("Error adding author");

            A.CallTo(() => _mediator.Send(A<AddAuthorCommand>.That.Matches(command => command.Name == addAuthorDto.Name), default))
                .Returns(Task.FromResult(operationResult));

            var result = await _controller.AddAuthor(addAuthorDto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Error adding author", badRequestResult.Value);
        }

        [Test]
        public async Task AddAuthor_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var addAuthorDto = new AddAuthorDto { Name = "New Author" };
            A.CallTo(() => _mediator.Send(A<AddAuthorCommand>.That.Matches(command => command.Name == addAuthorDto.Name), default))
                .Throws(new Exception("Unexpected error"));

            var result = await _controller.AddAuthor(addAuthorDto);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while adding the author.", objectResult.Value);
        }
    }
}
