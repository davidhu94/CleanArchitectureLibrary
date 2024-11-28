using Application.Commands.BookCommands.UpdateBook;
using Moq;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;

namespace TestProject1.BookUnitTests
{
    public class UpdateBookCommandHandlerTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
        }

        [Test]
        public async Task UpdateBook_ShouldReturnTrue_WhenBookUpdated()
        {
            var command = new UpdateBookCommand(1, "Updated Book Title", "Updated Description", 1);
            var existingBook = new Book { Id = 1, Title = "Old Title", Description = "Old Description", AuthorId = 1 };
            _bookRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

            var handler = new UpdateBookCommandHandler(_bookRepositoryMock.Object);

            var result = await handler.Handle(command, default);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task UpdateBook_ShouldReturnTrue_WhenBookExists()
        {
            var command = new UpdateBookCommand(1, "New Book Title", "New Description", 1);
            var existingBook = new Book { Id = 1, Title = "Old Title", Description = "Old Description", AuthorId = 1 };
            _bookRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

            var handler = new UpdateBookCommandHandler(_bookRepositoryMock.Object);

            var result = await handler.Handle(command, default);

            Assert.IsTrue(result);

            _bookRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Book>(b => b.Id == 1 && b.Title == "New Book Title" && b.Description == "New Description")), Times.Once);
        }

        [Test]
        public async Task UpdateBook_ShouldReturnFalse_WhenBookNotFound()
        {
            var command = new UpdateBookCommand(999, "Nonexistent Book", "Nonexistent Description", 1);
            _bookRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Book)null);

            var handler = new UpdateBookCommandHandler(_bookRepositoryMock.Object);

            var result = await handler.Handle(command, default);

            Assert.IsFalse(result);
        }
    }
}
