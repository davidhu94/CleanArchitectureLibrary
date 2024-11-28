using Application.Queries.BookQueries.GetAll;
using Moq;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Models;

namespace TestProject1.BookUnitTests
{
    public class GetAllBooksQueryHandlerUnitTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
        }

        [Test]
        public async Task GetAllBooks_ShouldReturnAllBooks()
        {
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Description = "Description 1", AuthorId = 1 },
                new Book { Id = 2, Title = "Book 2", Description = "Description 2", AuthorId = 1 },
                new Book { Id = 3, Title = "Book 3", Description = "Description 3", AuthorId = 2 },
                new Book { Id = 4, Title = "Book 4", Description = "Description 4", AuthorId = 2 },
                new Book { Id = 5, Title = "Book 5", Description = "Description 5", AuthorId = 3 }
            };

            _bookRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(books);

            var query = new GetAllBooksQuery();
            var handler = new GetAllBooksQueryHandler(_bookRepositoryMock.Object);

            var result = await handler.Handle(query, default);

            Assert.AreEqual(5, result.Count);
        }

        [Test]
        public async Task GetAllBooks_ShouldReturnEmptyList_WhenDatabaseHasNoBooks()
        {
            _bookRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Book>());

            var query = new GetAllBooksQuery();
            var handler = new GetAllBooksQueryHandler(_bookRepositoryMock.Object);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }
    }
}
