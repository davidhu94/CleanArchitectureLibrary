using API.Controllers;
using Application.Books.BookCommands.DeleteBook;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.BookAPITests
{
    public class DeleteBookControllerTests
    {
        private IBookRepository _bookRepository;
        private IAuthorRepository _authorRepository;
        private IMediator _mediator;
        private ILogger<BookController> _logger;
        private BookController _controller;


        [SetUp]
        public void SetUp()
        {
            _bookRepository = A.Fake<IBookRepository>();
            _authorRepository = A.Fake<IAuthorRepository>();
            _mediator = A.Fake<IMediator>();
            _logger = A.Fake<ILogger<BookController>>();

            _controller = new BookController(_mediator, _logger);
        }

        [Test]
        public async Task DeleteBook_ReturnsBadRequest_WhenIdIsInvalid()
        {
            int invalidId = 0;

            var result = await _controller.DeleteBook(invalidId);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Book ID must be greater than zero.", badRequestResult.Value);
        }

        [Test]
        public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            int nonExistingBookId = 999;
            A.CallTo(() => _mediator.Send(A<DeleteBookCommand>.Ignored, default))
                .Returns(Task.FromResult(OperationResult<bool>.Failure("Book with ID 999 not found.")));

            var result = await _controller.DeleteBook(nonExistingBookId);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Book with ID 999 not found.", notFoundResult.Value);
        }

        [Test]
        public async Task DeleteBook_ReturnsNoContent_WhenBookIsSuccessfullyDeleted()
        {
            int existingBookId = 1;
            A.CallTo(() => _mediator.Send(A<DeleteBookCommand>.Ignored, default))
                .Returns(Task.FromResult(OperationResult<bool>.Success(true, "Book deleted successfully.")));

            var result = await _controller.DeleteBook(existingBookId);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

    }
}
