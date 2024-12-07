using Application.Interfaces.RepositoryInterfaces;
using Application.Users.UserCommands.UpdateUser;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.UserUnitTests
{
    [TestFixture]
    public class UpdateUserCommandHandlerTests
    {
        private Mock<IUserRepository> _repositoryMock;
        private Mock<ILogger<UpdateUserCommandHandler>> _loggerMock;
        private UpdateUserCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UpdateUserCommandHandler>>();
            _handler = new UpdateUserCommandHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsInvalid()
        {
            var command = new UpdateUserCommand(0, "newUserName", "newPassword123");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Invalid user ID.", result.Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenNeitherUserNameNorPasswordIsProvided()
        {
            var command = new UpdateUserCommand(1, "", "");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("At least one field (UserName or Password) must be updated.", result.Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
        {
            var command = new UpdateUserCommand(1, "newUserName", "newPassword123");
            _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("User with ID 1 not found.", result.Message);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsUpdatedSuccessfully()
        {
            var command = new UpdateUserCommand(1, "newUserName", "newPassword123");
            var existingUser = new User { Id = 1, UserName = "oldUserName", PasswordHash = "oldPasswordHash" };
            _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);
            _repositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("User updated successfully.", result.Message);
            _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccursDuringUpdate()
        {
            var command = new UpdateUserCommand(1, "newUserName", "newPassword123");
            _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User());
            _repositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("An error occurred while updating user with ID 1: Database error", result.Message);
        }
    }
}
