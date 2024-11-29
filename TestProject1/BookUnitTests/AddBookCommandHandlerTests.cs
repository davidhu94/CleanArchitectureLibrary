using Application.Commands.BookCommands.AddBook;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Moq;

namespace TestProject1.BookUnitTests
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
        public async Task AddBook_ShouldReturnNewBookId_WhenValidData()
        {
            var command = new AddBookCommand("New Book Title", "This is a book description.", 1);
            var handler = new AddBookCommandHandler(_bookRepositoryMock.Object, _authorRepositoryMock.Object);

            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(new Author { Id = 1, Name = "Existing Author" });

            var newBook = new Book { Id = 1, Title = command.Title, Description = command.Description, AuthorId = command.AuthorId };
            _bookRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Book>()))
                .Callback<Book>(book => book.Id = 1)
                .Returns(Task.CompletedTask);

            var result = await handler.Handle(command, default);

            Assert.That(result, Is.GreaterThan(0));

            _bookRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Book>()), Times.Once);
        }

        [Test]
        public void AddBook_ShouldThrowArgumentException_WhenTitleOrDescriptionIsMissing()
        {
            var commandWithEmptyTitle = new AddBookCommand("", "This is a description.", 1);
            var commandWithEmptyDescription = new AddBookCommand("Title", "", 1);
            var commandWithNullTitle = new AddBookCommand(null, "This is a description.", 1);
            var commandWithNullDescription = new AddBookCommand("Title", null, 1);
            var handler = new AddBookCommandHandler(_bookRepositoryMock.Object, _authorRepositoryMock.Object);

            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(commandWithEmptyTitle, default));
            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(commandWithEmptyDescription, default));
            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(commandWithNullTitle, default));
            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(commandWithNullDescription, default));
        }

        [Test]
        public void AddBook_ShouldThrowArgumentException_WhenAuthorDoesNotExist()
        {
            var command = new AddBookCommand("New Book Title", "This is a book description.", 1);
            var handler = new AddBookCommandHandler(_bookRepositoryMock.Object, _authorRepositoryMock.Object);

            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync((Author)null);

            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, default));
        }
    }
}
