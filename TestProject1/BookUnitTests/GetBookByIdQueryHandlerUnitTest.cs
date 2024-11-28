using Application.Queries.BookQueries.GetById;
using Moq;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;

namespace TestProject1.BookUnitTests
{
    public class GetBookByIdQueryHandlerUnitTest
    {
        private Mock<IBookRepository> _bookRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
        }

        [Test]
        public async Task GetBookById_ShouldReturnBook_WhenBookExists()
        {
            var existingBookId = 1;
            var book = new Book { Id = existingBookId, Title = "Book 1", Description = "Description 1", AuthorId = 1 };
            _bookRepositoryMock.Setup(x => x.GetByIdAsync(existingBookId)).ReturnsAsync(book);

            var handler = new GetBookByIdQueryHandler(_bookRepositoryMock.Object);

            var result = await handler.Handle(new GetBookByIdQuery(existingBookId), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Id, Is.EqualTo(existingBookId));
            Assert.That(result?.Title, Is.EqualTo(book.Title));
        }

        [Test]
        public async Task GetBookById_ShouldReturnNull_WhenBookDoesNotExist()
        {
            var nonExistingBookId = 999;
            _bookRepositoryMock.Setup(x => x.GetByIdAsync(nonExistingBookId)).ReturnsAsync((Book)null);

            var handler = new GetBookByIdQueryHandler(_bookRepositoryMock.Object);

            var result = await handler.Handle(new GetBookByIdQuery(nonExistingBookId), CancellationToken.None);

            Assert.That(result, Is.Null);
        }
    }
}
