using Application.Interfaces.RepositoryInterfaces;
using Application.Users.UserCommands.AddUser;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.UserUnitTests
{
    [TestFixture]
    public class AddUserCommandHandlerTests
    {
        private Mock<IUserRepository> _repositoryMock;
        private Mock<ILogger<AddUserCommandHandler>> _loggerMock;
        private AddUserCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<AddUserCommandHandler>>();
            _handler = new AddUserCommandHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenUserNameIsEmpty()
        {
            var command = new AddUserCommand("", "password123");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("User Name is required.", result.Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenPasswordHashIsEmpty()
        {
            var command = new AddUserCommand("username", "");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Password is required.", result.Message);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccursDuringAdd()
        {
            var command = new AddUserCommand("username", "password123");
            _repositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                           .ThrowsAsync(new Exception("Database error"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("An error occurred while adding the user: Database error", result.Message);
        }
    }
}
