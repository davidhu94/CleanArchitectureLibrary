using Application.Commands.AuthorCommands.DeleteAuthor;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Moq;

namespace TestProject1.AuthorUnitTests
{
    public class DeleteAuthorCommandHandlerTests
    {
        private Mock<IAuthorRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAuthorRepository>();
        }

        [Test]
        public async Task DeleteAuthor_ShouldReturnTrue_WhenAuthorExists()
        {
            var command = new DeleteAuthorCommand(1);
            var handler = new DeleteAuthorCommandHandler(_repositoryMock.Object);

            _repositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Author { Id = 1, Name = "David" });

            _repositoryMock.Setup(r => r.DeleteAsync(1))
                .ReturnsAsync(true);

            var result = await handler.Handle(command, default);

            Assert.IsTrue(result);
            _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Test]
        public async Task DeleteAuthor_ShouldReturnFalse_WhenAuthorDoesNotExist()
        {
            var command = new DeleteAuthorCommand(999);
            var handler = new DeleteAuthorCommandHandler(_repositoryMock.Object);

            _repositoryMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Author)null);

            var result = await handler.Handle(command, default);

            Assert.IsFalse(result);
            _repositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once); 
            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
