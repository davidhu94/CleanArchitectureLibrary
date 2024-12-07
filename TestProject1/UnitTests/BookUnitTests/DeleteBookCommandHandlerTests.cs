using Application.Books.BookCommands.DeleteBook;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.BookUnitTests
{
    public class DeleteBookCommandHandlerTests
    {
        private Mock<IBookRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IBookRepository>();
        }

        [Test]
        public async Task DeleteBook_ShouldReturnSuccess_WhenBookExists()
        {
            var existingBookId = 1;
            var existingBook = new Book { Id = existingBookId, Title = "Sample Book", Description = "Sample Description" };

            _repositoryMock.Setup(r => r.GetByIdAsync(existingBookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBook);

            _repositoryMock.Setup(r => r.DeleteAsync(existingBookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var command = new DeleteBookCommand(existingBookId);
            var handler = new DeleteBookCommandHandler(_repositoryMock.Object, Mock.Of<ILogger<DeleteBookCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("Book deleted successfully.", result.Message);

            _repositoryMock.Verify(r => r.DeleteAsync(existingBookId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DeleteBook_ShouldReturnFailure_WhenBookDoesNotExist()
        {
            var nonExistingBookId = 999;

            _repositoryMock.Setup(r => r.GetByIdAsync(nonExistingBookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book)null);

            _repositoryMock.Setup(r => r.DeleteAsync(nonExistingBookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var command = new DeleteBookCommand(nonExistingBookId);
            var handler = new DeleteBookCommandHandler(_repositoryMock.Object, Mock.Of<ILogger<DeleteBookCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual($"Book with ID {nonExistingBookId} not found.", result.Message);

            _repositoryMock.Verify(r => r.DeleteAsync(nonExistingBookId, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task DeleteBook_ShouldReturnFailure_WhenInvalidId()
        {
            var invalidBookId = -1;
            var command = new DeleteBookCommand(invalidBookId);
            var handler = new DeleteBookCommandHandler(_repositoryMock.Object, Mock.Of<ILogger<DeleteBookCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Invalid book ID.", result.Message);

            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
