using Application.Authors.AuthorQueries.GetAll;
using Application.DTOs.AuthorDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.AuthorUnitTests
{
    public class GetAllAuthorsQueryHandlerUnitTests
    {
        private Mock<IAuthorRepository> _repositoryMock;
        private Mock<IMemoryCache> _memoryCacheMock;
        private Mock<ILogger<GetAllAuthorsQueryHandler>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAuthorRepository>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _loggerMock = new Mock<ILogger<GetAllAuthorsQueryHandler>>();
        }

        [Test]
        public async Task Handle_ShouldReturnAuthorsFromRepository_WhenNotInCache()
        {
            var authorsFromRepo = new List<Author>
            {
                new Author { Id = 1, Name = "Author 1" },
                new Author { Id = 2, Name = "Author 2" }
            };

            var authorDtos = authorsFromRepo.Select(a => new AuthorDto { Id = a.Id, Name = a.Name }).ToList();

            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(authorsFromRepo);

            _memoryCacheMock
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Returns(false);

            var cacheEntryMock = new Mock<ICacheEntry>();
            _memoryCacheMock
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            var command = new GetAllAuthorsQuery();
            var handler = new GetAllAuthorsQueryHandler(_repositoryMock.Object, _memoryCacheMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("Authors retrieved successfully.", result.Message);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual("Author 1", result.Data[0].Name);
            Assert.AreEqual("Author 2", result.Data[1].Name);

            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            _memoryCacheMock.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
        }
    }
}
