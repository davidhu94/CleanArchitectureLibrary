using Application.Interfaces.RepositoryInterfaces;
using Application.Users.UserQueries.GetUserByUsername;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.UserUnitTests
{
    public class GetUserByUsernameQueryHandlerTests
    {
        private Mock<IUserRepository> _repositoryMock;
        private Mock<ILogger<GetUserByUsernameQueryHandler>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<GetUserByUsernameQueryHandler>>();
        }

        [Test]
        public async Task Handle_ShouldReturnUser_WhenUserExists()
        {
            var userName = "existingUser";
            var existingUser = new User { Id = 1, UserName = userName, PasswordHash = "hash1" };

            _repositoryMock.Setup(r => r.GetByUsernameAsync(userName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            var command = new GetUserByUsernameQuery(userName);
            var handler = new GetUserByUsernameQueryHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(existingUser.UserName, result.UserName);
            Assert.AreEqual(existingUser.PasswordHash, result.PasswordHash);

            _repositoryMock.Verify(r => r.GetByUsernameAsync(userName, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var userName = "nonExistingUser";

            _repositoryMock.Setup(r => r.GetByUsernameAsync(userName, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var command = new GetUserByUsernameQuery(userName);
            var handler = new GetUserByUsernameQueryHandler(_repositoryMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsNull(result);

            _repositoryMock.Verify(r => r.GetByUsernameAsync(userName, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldThrowException_WhenErrorOccurs()
        {
            var userName = "errorUser";

            _repositoryMock.Setup(r => r.GetByUsernameAsync(userName, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var command = new GetUserByUsernameQuery(userName);
            var handler = new GetUserByUsernameQueryHandler(_repositoryMock.Object, _loggerMock.Object);

            var ex = Assert.ThrowsAsync<ApplicationException>(async () => await handler.Handle(command, CancellationToken.None));
            Assert.AreEqual($"An error occurred while fetching user with username {userName}.", ex.Message);

            _repositoryMock.Verify(r => r.GetByUsernameAsync(userName, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
