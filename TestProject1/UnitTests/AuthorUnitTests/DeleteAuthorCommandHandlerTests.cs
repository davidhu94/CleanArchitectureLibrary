using Application.Authors.AuthorCommands.DeleteAuthor;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.AuthorUnitTests
{
    [TestFixture]
    public class DeleteAuthorCommandHandlerTests
    {
        private Mock<IAuthorRepository> _repositoryMock;
        private Mock<ILogger<DeleteAuthorCommandHandler>> _loggerMock;
        private DeleteAuthorCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAuthorRepository>();
            _loggerMock = new Mock<ILogger<DeleteAuthorCommandHandler>>();
            _handler = new DeleteAuthorCommandHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenAuthorIsDeletedSuccessfully()
        {
            var authorId = 1;
            var command = new DeleteAuthorCommand(authorId);
            var cancellationToken = CancellationToken.None;

            _repositoryMock.Setup(r => r.GetByIdAsync(authorId, cancellationToken))
                .ReturnsAsync(new Author { Id = authorId });

            _repositoryMock.Setup(r => r.DeleteAsync(authorId, cancellationToken))
                .ReturnsAsync(true);

            var result = await _handler.Handle(command, cancellationToken);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("Author deleted successfully.", result.Message);

            _repositoryMock.Verify(r => r.GetByIdAsync(authorId, cancellationToken), Times.Once);
            _repositoryMock.Verify(r => r.DeleteAsync(authorId, cancellationToken), Times.Once);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Successfully deleted author with ID: {authorId}")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenAuthorDoesNotExist()
        {
            var authorId = 999;
            var command = new DeleteAuthorCommand(authorId);
            var cancellationToken = CancellationToken.None;

            _repositoryMock.Setup(r => r.GetByIdAsync(authorId, cancellationToken))
                .ReturnsAsync((Author)null);

            var result = await _handler.Handle(command, cancellationToken);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual($"Author with ID {authorId} not found.", result.Message);
            Assert.AreEqual($"Author with ID {authorId} not found.", result.ErrorMessage);
        }
    }
}
