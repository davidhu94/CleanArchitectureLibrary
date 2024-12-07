using Application.Books.BookCommands.UpdateBook;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.BookUnitTests
{
    public class UpdateBookCommandHandlerTests
    {
        private Mock<IBookRepository> _repositoryMock;
        private Mock<ILogger<UpdateBookCommandHandler>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IBookRepository>();
            _loggerMock = new Mock<ILogger<UpdateBookCommandHandler>>();
        }

        [Test]
        public async Task UpdateBook_ShouldReturnTrue_WhenBookExists()
        {
            var existingBookId = 1;
            var existingBook = new Book { Id = existingBookId, Title = "Old Title", Description = "Old Description", AuthorId = 1 };

            _repositoryMock.Setup(r => r.GetByIdAsync(existingBookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBook);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var command = new UpdateBookCommand(existingBookId, "Updated Title", "Updated Description", 1);
            var handler = new UpdateBookCommandHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("Updated Title", existingBook.Title);
            Assert.AreEqual("Updated Description", existingBook.Description);
            Assert.AreEqual(1, existingBook.AuthorId);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateBook_ShouldReturnFailure_WhenBookNotFound()
        {
            var nonExistingBookId = 999;

            _repositoryMock.Setup(r => r.GetByIdAsync(nonExistingBookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null);

            var command = new UpdateBookCommand(nonExistingBookId, "Updated Title", "Updated Description", 1);
            var handler = new UpdateBookCommandHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual($"Book with ID {nonExistingBookId} not found.", result.ErrorMessage);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task UpdateBook_ShouldReturnFailure_WhenInvalidBookId()
        {
            var invalidBookId = 0;

            var command = new UpdateBookCommand(invalidBookId, "Updated Title", "Updated Description", 1);
            var handler = new UpdateBookCommandHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Book ID must be greater than zero.", result.ErrorMessage);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task UpdateBook_ShouldReturnFailure_WhenInvalidAuthorId()
        {
            var validBookId = 1;
            var invalidAuthorId = 0;

            var command = new UpdateBookCommand(validBookId, "Updated Title", "Updated Description", invalidAuthorId);
            var handler = new UpdateBookCommandHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("A valid author ID is required.", result.ErrorMessage);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task UpdateBook_ShouldReturnFailure_WhenTitleIsEmpty()
        {
            var existingBookId = 1;
            var existingBook = new Book { Id = existingBookId, Title = "Old Title", Description = "Old Description", AuthorId = 1 };

            _repositoryMock.Setup(r => r.GetByIdAsync(existingBookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBook);

            var command = new UpdateBookCommand(existingBookId, "", "Updated Description", 1);
            var handler = new UpdateBookCommandHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Book title is required.", result.ErrorMessage);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
