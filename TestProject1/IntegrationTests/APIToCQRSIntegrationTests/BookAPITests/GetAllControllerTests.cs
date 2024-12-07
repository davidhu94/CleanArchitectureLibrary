using API.Controllers;
using Application.Books.BookQueries.GetAll;
using Application.DTOs.BookDTOs;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.BookAPITests
{
    public class GetAllBooksControllerTests
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
        public async Task GetAllBooks_ReturnsOk_WhenBooksAreFetchedSuccessfully()
        {
            var bookDtos = new List<BookDto>
            {
                new BookDto { Id = 1, Title = "Book 1" },
                new BookDto { Id = 2, Title = "Book 2" }
            };
            var operationResult = OperationResult<List<BookDto>>.Success(bookDtos, "Books fetched successfully");

            A.CallTo(() => _mediator.Send(A<GetAllBooksQuery>.That.Matches(query => true), default))
                .Returns(Task.FromResult(operationResult));

            var result = await _controller.GetAllBooks();

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var returnedBooks = okResult.Value as List<BookDto>;
            Assert.AreEqual(bookDtos.Count, returnedBooks.Count);
        }

        [Test]
        public async Task GetAllBooks_ReturnsInternalServerError_WhenFetchingBooksFails()
        {
            var operationResult = OperationResult<List<BookDto>>.Failure("An error occurred while fetching books");

            A.CallTo(() => _mediator.Send(A<GetAllBooksQuery>.That.Matches(query => true), default))
                .Returns(Task.FromResult(operationResult));

            var result = await _controller.GetAllBooks();

            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An error occurred while fetching books", objectResult.Value);
        }

        [Test]
        public async Task GetAllBooks_ReturnsBooksFromCache_WhenBooksAreCached()
        {
            var cachedBooks = new List<BookDto>
    {
        new BookDto { Id = 1, Title = "Book 1" },
        new BookDto { Id = 2, Title = "Book 2" }
    };

            var cacheKey = "AllBooks";
            object fakeCacheValue = cachedBooks;

            A.CallTo(() => _memoryCache.TryGetValue(cacheKey, out fakeCacheValue))
                .Returns(true);

            var operationResult = OperationResult<List<BookDto>>.Success(cachedBooks, "Books fetched successfully");

            A.CallTo(() => _mediator.Send(A<GetAllBooksQuery>.Ignored, default))
                .Returns(Task.FromResult(operationResult));

            var result = await _controller.GetAllBooks();

            Assert.IsInstanceOf<OkObjectResult>(result, "Expected an OkObjectResult");
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult?.Value, "The returned value should not be null");
            var returnedBooks = okResult.Value as List<BookDto>;

            Assert.AreEqual(cachedBooks.Count, returnedBooks.Count, "The count of cached books should match");
            Assert.AreEqual(cachedBooks[0].Title, returnedBooks[0].Title, "The titles of the first book should match");
        }


        [Test]
        public async Task GetAllBooks_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            A.CallTo(() => _mediator.Send(A<GetAllBooksQuery>.Ignored, default))
                .Throws(new Exception("Something went wrong"));

            var result = await _controller.GetAllBooks();

            Assert.IsInstanceOf<ObjectResult>(result, "Expected an ObjectResult when an exception occurs");
            var objectResult = result as ObjectResult;

            Assert.AreEqual(500, objectResult?.StatusCode, "Status code should be 500 for an internal server error");
            Assert.AreEqual("Something went wrong", objectResult?.Value, "The error message should match the exception message");
        }


    }
}
