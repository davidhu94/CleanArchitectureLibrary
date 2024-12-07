using API.Controllers;
using Application.Authors.AuthorQueries.GetById;
using Application.DTOs.AuthorDTOs;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.AuthorAPITests
{
    public class GetAuthorControllerTests
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
        public async Task GetAuthorById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            int invalidId = -1;

            var result = await _controller.GetAuthorById(invalidId);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Author ID must be greater than zero.", badRequestResult.Value);
        }

        [Test]
        public async Task GetAuthorById_ReturnsOk_WhenAuthorIsFound()
        {
            int validId = 1;
            var authorDto = new AuthorDto { Id = validId, Name = "John Doe" };
            var operationResult = OperationResult<AuthorDto>.Success(authorDto);
            A.CallTo(() => _mediator.Send(A<GetAuthorByIdQuery>.That.Matches(query => query.Id == validId), default))
                .Returns(Task.FromResult(operationResult));

            var result = await _controller.GetAuthorById(validId);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var returnedAuthor = okResult.Value as AuthorDto;
            Assert.AreEqual(validId, returnedAuthor.Id);
            Assert.AreEqual("John Doe", returnedAuthor.Name);
        }

        [Test]
        public async Task GetAuthorById_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            int validId = 1;
            var operationResult = OperationResult<AuthorDto>.Failure("Author not found");
            A.CallTo(() => _mediator.Send(A<GetAuthorByIdQuery>.That.Matches(query => query.Id == validId), default))
                .Returns(Task.FromResult(operationResult));

            var result = await _controller.GetAuthorById(validId);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Author not found", notFoundResult.Value);
        }

        [Test]
        public async Task GetAuthorById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            int validId = 1;
            A.CallTo(() => _mediator.Send(A<GetAuthorByIdQuery>.That.Matches(query => query.Id == validId), default))
                .Throws(new Exception("Something went wrong"));

            var result = await _controller.GetAuthorById(validId);

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while fetching the author.", objectResult.Value);
        }
    }
}
