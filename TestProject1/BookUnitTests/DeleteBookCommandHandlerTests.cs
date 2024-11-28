using Application.Commands.BookCommands.DeleteBook;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Moq;

namespace TestProject1.BookUnitTests
{
    public class DeleteBookCommandHandlerTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
        }

        [Test]
        public async Task DeleteBook_ShouldReturnTrue_WhenBookExists()
        {
            var bookIdToDelete = 1;
            var command = new DeleteBookCommand(bookIdToDelete);

            _bookRepositoryMock.Setup(x => x.GetByIdAsync(bookIdToDelete)).ReturnsAsync(new Book { Id = bookIdToDelete });

            var handler = new DeleteBookCommandHandler(_bookRepositoryMock.Object);

            var result = await handler.Handle(command, default);

            Assert.That(result, Is.True);
            _bookRepositoryMock.Verify(x => x.DeleteAsync(bookIdToDelete), Times.Once);
        }

        [Test]
        public async Task DeleteBook_ShouldReturnFalse_WhenBookDoesNotExist()
        {
            var bookIdToDelete = 999;
            var command = new DeleteBookCommand(bookIdToDelete);

            _bookRepositoryMock.Setup(x => x.GetByIdAsync(bookIdToDelete)).ReturnsAsync((Book)null);

            var handler = new DeleteBookCommandHandler(_bookRepositoryMock.Object);

            var result = await handler.Handle(command, default);

            Assert.That(result, Is.False);
        }

        [Test]
        public void DeleteBook_ShouldThrowArgumentException_WhenInvalidId()
        {
            var commandWithInvalidId = new DeleteBookCommand(0);

            var handler = new DeleteBookCommandHandler(_bookRepositoryMock.Object);

            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(commandWithInvalidId, default));
        }
    }
}
