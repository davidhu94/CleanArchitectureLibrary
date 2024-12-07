using API.Controllers;
using Application.Books.BookCommands.AddBook;
using Application.DTOs.BookDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestProject1.IntegrationTests.APIToCQRSIntegrationTests.BookAPITests
{
    [TestFixture]
    public class BookControllerTests
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
        public async Task AddBook_ReturnsBadRequest_WhenBookDataIsInvalid()
        {
            var addBookDto = new AddBookDto
            {
                Title = "",
                Description = "A great book",
                AuthorId = 1
            };

            var result = await _controller.AddBook(addBookDto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Title is required.", badRequestResult.Value);
        }

        [Test]
        public async Task AddBook_ReturnsBadRequest_WhenAuthorDoesNotExist()
        {
            var addBookDto = new AddBookDto
            {
                Title = "New Book",
                Description = "A great book",
                AuthorId = 999
            };

            A.CallTo(() => _authorRepository.GetByIdAsync(999, default)).Returns(Task.FromResult<Author>(null));

            var operationResult = OperationResult<BookDto>.Failure("Author with ID 999 does not exist.");
            A.CallTo(() => _mediator.Send(A<AddBookCommand>.Ignored, default)).Returns(Task.FromResult(operationResult));

            var result = await _controller.AddBook(addBookDto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var badRequestResult = result as BadRequestObjectResult;

            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            Assert.AreEqual("Author with ID 999 does not exist.", badRequestResult.Value);
        }




        [Test]
        public async Task AddBook_ReturnsCreated_WhenBookIsSuccessfullyAdded()
        {
            var addBookDto = new AddBookDto
            {
                Title = "New Book",
                Description = "A great book",
                AuthorId = 1
            };

            var author = new Author { Id = 1, Name = "Existing Author" };
            A.CallTo(() => _authorRepository.GetByIdAsync(1, default)).Returns(Task.FromResult(author));

            var newBook = new Book { Id = 1, Title = addBookDto.Title, Description = addBookDto.Description, AuthorId = addBookDto.AuthorId };
            var bookDto = new BookDto { Id = 1, Title = addBookDto.Title, Description = addBookDto.Description, AuthorId = addBookDto.AuthorId };

            A.CallTo(() => _bookRepository.AddAsync(A<Book>.Ignored, default)).Returns(Task.CompletedTask);

            var addBookCommand = new AddBookCommand(addBookDto.Title, addBookDto.Description, addBookDto.AuthorId);
            var operationResult = OperationResult<BookDto>.Success(bookDto, "Book added successfully.");
            A.CallTo(() => _mediator.Send(A<AddBookCommand>.Ignored, default)).Returns(Task.FromResult(operationResult));

            var result = await _controller.AddBook(addBookDto);

            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            Assert.AreEqual("GetBookById", createdResult.ActionName);
            Assert.AreEqual(bookDto, createdResult.Value);
        }
    }
}
