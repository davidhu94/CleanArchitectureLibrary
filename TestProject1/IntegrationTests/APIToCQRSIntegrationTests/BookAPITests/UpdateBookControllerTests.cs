using API.Controllers;
using Application.Books.BookCommands.UpdateBook;
using Application.DTOs.BookDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.BookAPITests
{
    public class UpdateBookControllerTests
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
        public async Task UpdateBook_ReturnsBadRequest_WhenBookDataIsInvalid()
        {
            var invalidBookDto = new UpdateBookDto
            {
                Id = 0,
                Title = "",
                Description = "",
                AuthorId = 0
            };

            var result = await _controller.UpdateBook(0, invalidBookDto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Book ID must be greater than zero.", badRequestResult.Value);
        }


        [Test]
        public async Task UpdateBook_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            var updateBookDto = new UpdateBookDto
            {
                Id = 1,
                Title = "Updated Book Title",
                Description = "Updated Description",
                AuthorId = 1
            };

            var result = await _controller.UpdateBook(999, updateBookDto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("The Id in the URL must match the book Id in the body.", badRequestResult.Value);
        }

        [Test]
        public async Task UpdateBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            var updateBookDto = new UpdateBookDto
            {
                Id = 1,
                Title = "Updated Book Title",
                Description = "Updated Description",
                AuthorId = 1
            };

            A.CallTo(() => _mediator.Send(A<UpdateBookCommand>.Ignored, default))
                .Returns(Task.FromResult(OperationResult<bool>.Failure("Book with ID 1 not found.")));

            var result = await _controller.UpdateBook(1, updateBookDto);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Book with ID 1 not found.", notFoundResult.Value);
        }

        [Test]
        public async Task UpdateBook_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            var updateBookDto = new UpdateBookDto
            {
                Id = 1,
                Title = "Updated Book Title",
                Description = "Updated Description",
                AuthorId = 1
            };

            var existingBook = new Book
            {
                Id = 1,
                Title = "Original Title",
                Description = "Original Description",
                AuthorId = 1
            };

            A.CallTo(() => _bookRepository.GetByIdAsync(1, default)).Returns(Task.FromResult(existingBook));

            A.CallTo(() => _bookRepository.UpdateAsync(existingBook, default)).Returns(Task.CompletedTask);

            A.CallTo(() => _mediator.Send(A<UpdateBookCommand>.Ignored, default))
                .Returns(Task.FromResult(OperationResult<bool>.Success(true, "Book updated successfully.")));

            var result = await _controller.UpdateBook(1, updateBookDto);

            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
