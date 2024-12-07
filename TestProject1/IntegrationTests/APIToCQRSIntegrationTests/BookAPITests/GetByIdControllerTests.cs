using API.Controllers;
using Application.Books.BookQueries.GetById;
using Application.DTOs.BookDTOs;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.BookAPITests
{
    public class GetByIdControllerTests
    {
        private IMediator _mediator;
        private ILogger<BookController> _logger;
        private IMemoryCache _memoryCache;
        private BookController _controller;

        [SetUp]
        public void Setup()
        {
            _mediator = A.Fake<IMediator>();
            _logger = A.Fake<ILogger<BookController>>();
            _memoryCache = A.Fake<IMemoryCache>();
            _controller = new BookController(_mediator, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _memoryCache?.Dispose();
        }

        [Test]
        public async Task GetBookById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            int invalidId = 0;

            var result = await _controller.GetBookById(invalidId);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Book ID must be greater than zero.", badRequestResult.Value);
        }

        [Test]
        public async Task GetBookById_ReturnsNotFound_WhenBookDoesNotExist()
        {
            int nonExistingBookId = 999;
            A.CallTo(() => _mediator.Send(A<GetBookByIdQuery>.Ignored, default))
                .Returns(Task.FromResult(OperationResult<BookDto>.Failure("Book with ID 999 was not found.")));

            var result = await _controller.GetBookById(nonExistingBookId);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Book with ID 999 was not found.", notFoundResult.Value);
        }

        [Test]
        public async Task GetBookById_ReturnsOk_WhenBookIsFound()
        {
            int existingBookId = 1;
            var existingBook = new Book { Id = existingBookId, Title = "Existing Book", Description = "A great book", AuthorId = 1 };
            var bookDto = new BookDto { Id = existingBookId, Title = existingBook.Title, Description = existingBook.Description, AuthorId = existingBook.AuthorId };

            A.CallTo(() => _mediator.Send(A<GetBookByIdQuery>.Ignored, default))
                .Returns(Task.FromResult(OperationResult<BookDto>.Success(bookDto, "Book fetched successfully.")));

            var result = await _controller.GetBookById(existingBookId);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(bookDto, okResult.Value);
        }

        [Test]
        public async Task GetBookById_ReturnsOk_WhenBookIsFetchedFromCache()
        {
            int existingBookId = 1;
            var cachedBookDto = new BookDto { Id = existingBookId, Title = "Cached Book", Description = "A cached book", AuthorId = 1 };

            A.CallTo(() => _mediator.Send(A<GetBookByIdQuery>.Ignored, default))
                .Returns(Task.FromResult(OperationResult<BookDto>.Success(cachedBookDto, "Book fetched from cache.")));

            var result = await _controller.GetBookById(existingBookId);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(cachedBookDto, okResult.Value);
        }
    }
}
