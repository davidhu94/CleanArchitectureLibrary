using Application.Interfaces.RepositoryInterfaces;
using Application.Users.UserCommands.DeleteUser;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.UserUnitTests
{
    public class DeleteUserCommandHandlerTests
    {
        private Mock<IUserRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUserRepository>();
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenUserExists()
        {
            var existingUserId = 1;
            var existingUser = new User { Id = existingUserId, UserName = "Test User", PasswordHash = "hashedPassword" };

            _repositoryMock.Setup(r => r.GetByIdAsync(existingUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            _repositoryMock.Setup(r => r.DeleteAsync(existingUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var command = new DeleteUserCommand(existingUserId);
            var handler = new DeleteUserCommandHandler(_repositoryMock.Object, Mock.Of<ILogger<DeleteUserCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("User deleted successfully.", result.Message);

            _repositoryMock.Verify(r => r.DeleteAsync(existingUserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            var nonExistingUserId = 999;

            _repositoryMock.Setup(r => r.GetByIdAsync(nonExistingUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            _repositoryMock.Setup(r => r.DeleteAsync(nonExistingUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var command = new DeleteUserCommand(nonExistingUserId);
            var handler = new DeleteUserCommandHandler(_repositoryMock.Object, Mock.Of<ILogger<DeleteUserCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual($"User with ID {nonExistingUserId} not found.", result.Message);

            _repositoryMock.Verify(r => r.DeleteAsync(nonExistingUserId, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenInvalidId()
        {
            var invalidUserId = -1;
            var command = new DeleteUserCommand(invalidUserId);
            var handler = new DeleteUserCommandHandler(_repositoryMock.Object, Mock.Of<ILogger<DeleteUserCommandHandler>>());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Invalid user ID.", result.Message);

            _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
