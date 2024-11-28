using Application.Commands.AuthorCommands.AddAuthor;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Moq;

namespace TestProject1.AuthorUnitTests
{
    public class AddAuthorCommandHandlerTests
    {
        private Mock<IAuthorRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAuthorRepository>();
        }

        [Test]
        public async Task AddAuthor_ShouldReturnNewAuthorId_WhenAuthorIsAddedSuccessfully()
        {
            var command = new AddAuthorCommand("David");
            var handler = new AddAuthorCommandHandler(_repositoryMock.Object);

            var result = await handler.Handle(command, default);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Once);
        }

        [Test]
        public void AddAuthor_ShouldThrowArgumentException_WhenNameIsNullOrWhitespace()
        {

            var command = new AddAuthorCommand(null);
            var handler = new AddAuthorCommandHandler(_repositoryMock.Object);

            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, default));

            command = new AddAuthorCommand(" ");

            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, default));
        }
    }
}
