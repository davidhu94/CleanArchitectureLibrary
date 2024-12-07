using Application.Interfaces.RepositoryInterfaces;
using Application.Users.UserQueries.GetById;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.UserUnitTests
{
    public class GetUserByIdQueryHandlerTests
    {
        private Mock<IUserRepository> _repositoryMock;
        private Mock<ILogger<GetUserByIdQueryHandler>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<GetUserByIdQueryHandler>>();
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenUserExists()
        {
            var userId = 1;
            var existingUser = new User { Id = userId, UserName = "user1", PasswordHash = "hash1" };

            _repositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            var command = new GetUserByIdQuery(userId);
            var handler = new GetUserByIdQueryHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(userId, result.Data.Id);
            Assert.AreEqual("user1", result.Data.UserName);

            _repositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            var userId = 999;

            _repositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var command = new GetUserByIdQuery(userId);
            var handler = new GetUserByIdQueryHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual($"User with ID {userId} was not found.", result.Message);

            _repositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
        {
            var userId = 1;

            _repositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var command = new GetUserByIdQuery(userId);
            var handler = new GetUserByIdQueryHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("An error occurred while fetching the user.", result.Message);

            _repositoryMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
