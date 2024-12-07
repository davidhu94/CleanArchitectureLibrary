using Application.Authors.AuthorQueries.GetById;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.UnitTests.AuthorUnitTests
{
    public class GetAuthorByIdQueryHandlerUnitTest
    {
        private Mock<IAuthorRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAuthorRepository>();
        }

        [Test]
        public async Task GetAuthorById_ShouldReturnAuthor_WhenAuthorExists()
        {
            var existingAuthorId = 1;
            var existingAuthor = new Author { Id = existingAuthorId, Name = "Author 1" };

            _repositoryMock.Setup(r => r.GetByIdAsync(existingAuthorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAuthor);

            var handler = new GetAuthorByIdQueryHandler(_repositoryMock.Object, Mock.Of<ILogger<GetAuthorByIdQueryHandler>>());

            var result = await handler.Handle(new GetAuthorByIdQuery(existingAuthorId), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccessful, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data?.Id, Is.EqualTo(existingAuthorId));
            Assert.That(result.Data?.Name, Is.EqualTo(existingAuthor.Name));
        }

        [Test]
        public async Task GetAuthorById_ShouldReturnNull_WhenAuthorDoesNotExist()
        {
            var nonExistingAuthorId = 999;

            _repositoryMock.Setup(r => r.GetByIdAsync(nonExistingAuthorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Author)null);

            var handler = new GetAuthorByIdQueryHandler(_repositoryMock.Object, Mock.Of<ILogger<GetAuthorByIdQueryHandler>>());

            var result = await handler.Handle(new GetAuthorByIdQuery(nonExistingAuthorId), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo($"Author with ID {nonExistingAuthorId} was not found."));
        }
    }
}
