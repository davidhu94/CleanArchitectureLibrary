using Application.Books.BookQueries.GetAll;
using Application.DTOs.BookDTOs;
using Application.Interfaces.RepositoryInterfaces;
using Application.Mappers;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestProject1.BookUnitTests
{
    public class GetAllBooksQueryHandlerTests
    {
        private Mock<IBookRepository> _repositoryMock;
        private Mock<IMemoryCache> _memoryCacheMock;
        private Mock<ILogger<GetAllBooksQueryHandler>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IBookRepository>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _loggerMock = new Mock<ILogger<GetAllBooksQueryHandler>>();
        }

        [Test]
        public async Task Handle_ShouldReturnBooksFromRepository_WhenNotInCache()
        {
            var booksFromRepo = new List<Book>
    {
        new Book { Id = 1, Title = "Book 1", Description = "Description 1" },
        new Book { Id = 2, Title = "Book 2", Description = "Description 2" }
    };

            var bookDtos = booksFromRepo.Select(BookMapper.ToDto).ToList();

            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(booksFromRepo);

            _memoryCacheMock
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Returns(false);

            var cacheEntryMock = new Mock<ICacheEntry>();
            _memoryCacheMock
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            var command = new GetAllBooksQuery();
            var handler = new GetAllBooksQueryHandler(_repositoryMock.Object, _memoryCacheMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("Books fetched successfully.", result.Message);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual("Book 1", result.Data[0].Title);
            Assert.AreEqual("Book 2", result.Data[1].Title);

            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            _memoryCacheMock.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
        }



        [Test]
        public async Task Handle_ShouldReturnBooksFromCache_WhenInCache()
        {
            var cachedBookDtos = new List<BookDto>
    {
        new BookDto { Id = 1, Title = "Cached Book 1", Description = "Cached Description 1" },
        new BookDto { Id = 2, Title = "Cached Book 2", Description = "Cached Description 2" }
    };

            _memoryCacheMock
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Returns((object key, out object value) =>
                {
                    value = cachedBookDtos;
                    return true;
                });

            var command = new GetAllBooksQuery();
            var handler = new GetAllBooksQueryHandler(_repositoryMock.Object, _memoryCacheMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("Books fetched from cache.", result.Message);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual("Cached Book 1", result.Data[0].Title);
            Assert.AreEqual("Cached Book 2", result.Data[1].Title);

            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Test]
        public async Task Handle_ShouldReturnFailure_WhenNoBooksFoundInRepository()
        {
            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Book>());

            _memoryCacheMock
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Returns(false);

            var command = new GetAllBooksQuery();
            var handler = new GetAllBooksQueryHandler(_repositoryMock.Object, _memoryCacheMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("No books found.", result.Message);

            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Test]
        public async Task Handle_ShouldReturnFailure_WhenErrorOccursInRepository()
        {
            _repositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            _memoryCacheMock
                .Setup(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Returns(false);

            var command = new GetAllBooksQuery();
            var handler = new GetAllBooksQueryHandler(_repositoryMock.Object, _memoryCacheMock.Object, _loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual("An error occurred while fetching books.", result.Message);

            _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
