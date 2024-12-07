using Application.Authors.AuthorCommands.AddAuthor;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.AuthorUnitTests
{
    [TestFixture]
    public class AddAuthorCommandHandlerTests
    {
        private Mock<IAuthorRepository> _repositoryMock;
        private Mock<ILogger<AddAuthorCommandHandler>> _loggerMock;
        private AddAuthorCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAuthorRepository>();
            _loggerMock = new Mock<ILogger<AddAuthorCommandHandler>>();
            _handler = new AddAuthorCommandHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnAuthorDto_WhenAuthorIsAddedSuccessfully()
        {
            var command = new AddAuthorCommand("David");
            var cancellationToken = CancellationToken.None;

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Author>(), cancellationToken))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, cancellationToken);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("David", result.Data.Name);

            _repositoryMock.Verify(r => r.AddAsync(It.Is<Author>(a => a.Name == "David"), cancellationToken), Times.Once);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Adding a new author with name: David")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenAuthorNameIsNullOrWhitespace()
        {
            var commandWithNullName = new AddAuthorCommand(null);
            var commandWithWhitespaceName = new AddAuthorCommand(" ");
            var cancellationToken = CancellationToken.None;

            var resultWithNullName = await _handler.Handle(commandWithNullName, cancellationToken);
            var resultWithWhitespaceName = await _handler.Handle(commandWithWhitespaceName, cancellationToken);

            Assert.IsFalse(resultWithNullName.IsSuccessful);
            Assert.AreEqual("Author name is required.", resultWithNullName.ErrorMessage);

            Assert.IsFalse(resultWithWhitespaceName.IsSuccessful);
            Assert.AreEqual("Author name is required.", resultWithWhitespaceName.ErrorMessage);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsException()
        {
            var command = new AddAuthorCommand("David");
            var cancellationToken = CancellationToken.None;

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Author>(), cancellationToken))
                .Throws(new Exception("Database error"));

            var result = await _handler.Handle(command, cancellationToken);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("An error occurred while adding the author.", result.ErrorMessage);
            Assert.AreEqual("Database error", result.Message);

            _repositoryMock.Verify(r => r.AddAsync(It.Is<Author>(a => a.Name == "David"), cancellationToken), Times.Once);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("An error occurred while adding the author")),
                    It.Is<Exception>(ex => ex.Message == "Database error"),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
