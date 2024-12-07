using Application.Books.BookQueries.GetById;
using Application.Interfaces.RepositoryInterfaces;
using Application.Mappers;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.BookUnitTests
{
    public class GetBookByIdQueryHandlerTests
    {
        private Mock<IBookRepository> _repositoryMock;
        private Mock<IMemoryCache> _cacheMock;
        private Mock<ILogger<GetBookByIdQueryHandler>> _loggerMock;
        private GetBookByIdQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IBookRepository>();
            _cacheMock = new Mock<IMemoryCache>();
            _loggerMock = new Mock<ILogger<GetBookByIdQueryHandler>>();
            _handler = new GetBookByIdQueryHandler(_repositoryMock.Object, _cacheMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnBookFromRepository_WhenNotInCache()
        {
            var bookFromRepo = new Book
            {
                Id = 1,
                Title = "Book 1",
                Description = "Description 1",
                AuthorId = 1
            };

            var bookDto = BookMapper.ToDto(bookFromRepo);

            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(bookFromRepo);

            _cacheMock.Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                            .Returns(false);

            var cacheEntryMock = new Mock<ICacheEntry>();
            _cacheMock.Setup(m => m.CreateEntry(It.IsAny<object>()))
                            .Returns(cacheEntryMock.Object);

            var query = new GetBookByIdQuery(1);
            var handler = new GetBookByIdQueryHandler(_repositoryMock.Object, _cacheMock.Object, _loggerMock.Object);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("Book fetched successfully.", result.Message);
            Assert.AreEqual(1, result.Data.Id);
            Assert.AreEqual("Book 1", result.Data.Title);
            Assert.AreEqual("Description 1", result.Data.Description);

            _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);

            _cacheMock.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnBadRequest_WhenBookIdIsInvalid()
        {
            var query = new GetBookByIdQuery(0);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("Book with ID 0 was not found.", result.Message);
        }

    }
}
