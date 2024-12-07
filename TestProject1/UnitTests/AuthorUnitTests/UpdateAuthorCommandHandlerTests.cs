using Application.Authors.AuthorCommands.UpdateAuthor;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.AuthorUnitTests
{
    public class UpdateAuthorCommandHandlerTests
    {
        private Mock<IAuthorRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAuthorRepository>();
        }

        [Test]
        public async Task UpdateAuthor_ShouldReturnTrue_WhenAuthorExists()
        {
            var existingAuthorId = 1;
            var existingAuthor = new Author { Id = existingAuthorId, Name = "Old Author Name" };

            _repositoryMock.Setup(r => r.GetByIdAsync(existingAuthorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAuthor);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var command = new UpdateAuthorCommand(existingAuthorId, "Updated Author Name");
            var handler = new UpdateAuthorCommandHandler(_repositoryMock.Object, Mock.Of<ILogger<UpdateAuthorCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("Updated Author Name", existingAuthor.Name);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateAuthor_ShouldReturnFalse_WhenAuthorNotFound()
        {
            var nonExistingAuthorId = 999;

            _repositoryMock.Setup(r => r.GetByIdAsync(nonExistingAuthorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Author)null);

            var command = new UpdateAuthorCommand(nonExistingAuthorId, "Nonexistent Author");
            var handler = new UpdateAuthorCommandHandler(_repositoryMock.Object, Mock.Of<ILogger<UpdateAuthorCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual($"Author with ID {nonExistingAuthorId} not found.", result.ErrorMessage);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task UpdateAuthor_ShouldReturnFailure_WhenInvalidIdIsProvided()
        {
            var invalidAuthorId = -1;

            var command = new UpdateAuthorCommand(invalidAuthorId, "Invalid Author Name");
            var handler = new UpdateAuthorCommandHandler(_repositoryMock.Object, Mock.Of<ILogger<UpdateAuthorCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Invalid author ID.", result.ErrorMessage);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
