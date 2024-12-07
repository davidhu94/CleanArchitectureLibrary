using Application.Books.BookCommands.AddBook;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.BookUnitTests
{
    public class AddBookCommandHandlerTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;
        private Mock<IAuthorRepository> _authorRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _authorRepositoryMock = new Mock<IAuthorRepository>();
        }

        [Test]
        public async Task AddBook_ShouldReturnSuccess_WhenBookIsValid()
        {
            var validAuthorId = 1;
            var validBookTitle = "Valid Book Title";
            var validBookDescription = "Valid Book Description";

            _authorRepositoryMock.Setup(r => r.GetByIdAsync(validAuthorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Author { Id = validAuthorId, Name = "Valid Author" });

            _bookRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var command = new AddBookCommand(validBookTitle, validBookDescription, validAuthorId);
            var handler = new AddBookCommandHandler(_bookRepositoryMock.Object, _authorRepositoryMock.Object, Mock.Of<ILogger<AddBookCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("Book added successfully.", result.Message);

            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task AddBook_ShouldReturnFailure_WhenTitleIsEmpty()
        {
            var invalidTitle = "";
            var validDescription = "Valid Description";
            var validAuthorId = 1;

            var command = new AddBookCommand(invalidTitle, validDescription, validAuthorId);
            var handler = new AddBookCommandHandler(_bookRepositoryMock.Object, _authorRepositoryMock.Object, Mock.Of<ILogger<AddBookCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Title is required.", result.ErrorMessage);

            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task AddBook_ShouldReturnFailure_WhenDescriptionIsEmpty()
        {
            var validTitle = "Valid Book Title";
            var invalidDescription = "";
            var validAuthorId = 1;

            var command = new AddBookCommand(validTitle, invalidDescription, validAuthorId);
            var handler = new AddBookCommandHandler(_bookRepositoryMock.Object, _authorRepositoryMock.Object, Mock.Of<ILogger<AddBookCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Description is required.", result.ErrorMessage);

            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task AddBook_ShouldReturnFailure_WhenAuthorDoesNotExist()
        {
            var validTitle = "Valid Book Title";
            var validDescription = "Valid Book Description";
            var invalidAuthorId = 999;

            _authorRepositoryMock.Setup(r => r.GetByIdAsync(invalidAuthorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Author)null);

            var command = new AddBookCommand(validTitle, validDescription, invalidAuthorId);
            var handler = new AddBookCommandHandler(_bookRepositoryMock.Object, _authorRepositoryMock.Object, Mock.Of<ILogger<AddBookCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual($"Author with ID {invalidAuthorId} does not exist.", result.ErrorMessage);

            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
