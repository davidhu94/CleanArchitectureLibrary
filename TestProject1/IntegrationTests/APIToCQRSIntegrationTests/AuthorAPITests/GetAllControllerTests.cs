using API.Controllers;
using Application.Authors.AuthorQueries.GetAll;
using Application.DTOs.AuthorDTOs;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.AuthorAPITests
{
    [TestFixture]
    public class GetAllControllerTests
    {
        private AuthorController _controller;
        private IMediator _mediator;
        private ILogger<AuthorController> _logger;

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
            if (_mediator is IDisposable disposableMediator)
            {
                disposableMediator.Dispose();
            }

            if (_logger is IDisposable disposableLogger)
            {
                disposableLogger.Dispose();
            }

            if (_controller is IDisposable disposableController)
            {
                disposableController.Dispose();
            }
        }

        [Test]
        public async Task GetAllAuthors_ReturnsOk_WhenAuthorsExist()
        {
            var authors = new List<AuthorDto>
            {
                new AuthorDto { Id = 1, Name = "Author One" },
                new AuthorDto { Id = 2, Name = "Author Two" }
            };

            A.CallTo(() => _mediator.Send(A<GetAllAuthorsQuery>._, default))
                .Returns(OperationResult<List<AuthorDto>>.Success(authors));

            var result = await _controller.GetAllAuthors();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnValue = okResult.Value as List<AuthorDto>;
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(2, returnValue.Count);
        }

        [Test]
        public async Task GetAllAuthors_ReturnsBadRequest_WhenQueryFails()
        {
            string errorMessage = "Failed to retrieve authors.";

            A.CallTo(() => _mediator.Send(A<GetAllAuthorsQuery>._, default))
                .Returns(OperationResult<List<AuthorDto>>.Failure(errorMessage));

            var result = await _controller.GetAllAuthors();

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(errorMessage, badRequestResult.Value);
        }

        [Test]
        public async Task GetAllAuthors_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var fakeMediator = A.Fake<IMediator>();
            var fakeLogger = A.Fake<ILogger<AuthorController>>();

            A.CallTo(() => fakeMediator.Send(A<GetAllAuthorsQuery>._, A<CancellationToken>._))
                .Throws(new Exception("Something went wrong"));

            var controller = new AuthorController(fakeMediator, fakeLogger);

            var result = await controller.GetAllAuthors();

            Assert.IsNotNull(result, "Result should not be null");
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult, "Result should be of type ObjectResult");
            Assert.AreEqual(500, objectResult.StatusCode, "Status code should be 500");
            Assert.AreEqual("An error occurred while fetching authors.", objectResult.Value, "Error message mismatch");
        }


    }
}
