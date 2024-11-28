using Application.Commands.AuthorCommands.UpdateAuthor;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Moq;

namespace TestProject1.AuthorUnitTests
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

            _repositoryMock.Setup(r => r.GetByIdAsync(existingAuthorId))
                .ReturnsAsync(existingAuthor);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Author>()))
                .Returns(Task.CompletedTask);

            var command = new UpdateAuthorCommand(existingAuthorId, "Updated Author Name");
            var handler = new UpdateAuthorCommandHandler(_repositoryMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual("Updated Author Name", existingAuthor.Name);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Author>()), Times.Once);
        }

        [Test]
        public async Task UpdateAuthor_ShouldReturnFalse_WhenAuthorNotFound()
        {
            var nonExistingAuthorId = 999;

            _repositoryMock.Setup(r => r.GetByIdAsync(nonExistingAuthorId))
                .ReturnsAsync((Author)null);

            var command = new UpdateAuthorCommand(nonExistingAuthorId, "Nonexistent Author");
            var handler = new UpdateAuthorCommandHandler(_repositoryMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result);

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Author>()), Times.Never);
        }
    }
}
