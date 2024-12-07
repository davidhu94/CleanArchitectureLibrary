using Application.Interfaces.RepositoryInterfaces;
using Application.Users.UserQueries.GetAll;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.UserUnitTests
{
    public class GetAllUsersQueryHandlerTests
    {
        private Mock<IUserRepository> _repositoryMock;
        private Mock<ILogger<GetAllUsersQueryHandler>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<GetAllUsersQueryHandler>>();
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenUsersExist()
        {
            var users = new List<User>
            {
                new User { Id = 1, UserName = "user1", PasswordHash = "hash1" },
                new User { Id = 2, UserName = "user2", PasswordHash = "hash2" }
            };

            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            var command = new GetAllUsersQuery();
            var handler = new GetAllUsersQueryHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual("user1", result.Data[0].UserName);
            Assert.AreEqual("user2", result.Data[1].UserName);

            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenNoUsersExist()
        {
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());

            var command = new GetAllUsersQuery();
            var handler = new GetAllUsersQueryHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("No users found.", result.Message);

            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenExceptionOccurs()
        {
            _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var command = new GetAllUsersQuery();
            var handler = new GetAllUsersQueryHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("An error occurred while fetching users.", result.Message);

            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
