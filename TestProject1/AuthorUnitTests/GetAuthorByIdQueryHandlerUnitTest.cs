using Application.Interfaces.RepositoryInterfaces;
using Application.Queries.AuthorQueries.GetById;
using Domain.Models;
using Moq;

namespace TestProject1.AuthorUnitTests
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

            _repositoryMock.Setup(r => r.GetByIdAsync(existingAuthorId))
                .ReturnsAsync(existingAuthor);

            var handler = new GetAuthorByIdQueryHandler(_repositoryMock.Object);

            var result = await handler.Handle(new GetAuthorByIdQuery(existingAuthorId), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Id, Is.EqualTo(existingAuthorId));
            Assert.That(result?.Name, Is.EqualTo(existingAuthor.Name));
        }

        [Test]
        public async Task GetAuthorById_ShouldReturnNull_WhenAuthorDoesNotExist()
        {
            var nonExistingAuthorId = 999;

            _repositoryMock.Setup(r => r.GetByIdAsync(nonExistingAuthorId))
                .ReturnsAsync((Author)null);

            var handler = new GetAuthorByIdQueryHandler(_repositoryMock.Object);

            var result = await handler.Handle(new GetAuthorByIdQuery(nonExistingAuthorId), CancellationToken.None);

            Assert.That(result, Is.Null);
        }
    }
}
