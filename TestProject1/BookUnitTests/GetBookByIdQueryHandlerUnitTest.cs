using Application.Queries.BookQueries.GetById;
using Infrastructure.Database;

namespace TestProject1.BookUnitTests
{
    public class GetBookByIdQueryHandlerUnitTest
    {
        private FakeDatabase _fakeDatabase;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
        }

        [Test]
        public async Task GetBookById_ShouldReturnBook_WhenBookExists()
        {
            var existingBookId = 1;
            var handler = new GetBookByIdQueryHandler(_fakeDatabase);

            var result = await handler.Handle(new GetBookByIdQuery(existingBookId), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Id, Is.EqualTo(existingBookId));
            Assert.That(result?.Title, Is.EqualTo(_fakeDatabase.Books.First(b => b.Id == existingBookId).Title));
        }

        [Test]
        public async Task GetBookById_ShouldReturnNull_WhenBookDoesNotExist()
        {
            var nonExistingBookId = 999;
            var handler = new GetBookByIdQueryHandler(_fakeDatabase);

            var result = await handler.Handle(new GetBookByIdQuery(nonExistingBookId), CancellationToken.None);

            Assert.That(result, Is.Null);
        }
    }
}
