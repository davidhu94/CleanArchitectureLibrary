using Application.Queries.BookQueries.GetAll;
using Infrastructure.Database;

namespace TestProject1.BookUnitTests
{
    public class GetAllBooksQueryHandlerUnitTests
    {
        private FakeDatabase _fakeDatabase;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
        }

        [Test]
        public async Task GetAllBooks_ShouldReturnAllBooks()
        {
            var query = new GetAllBooksQuery();
            var handler = new GetAllBooksQueryHandler(_fakeDatabase);

            var result = await handler.Handle(query, default);

            Assert.AreEqual(5, result.Count);
        }

        [Test]
        public async Task GetAllBooks_ShouldReturnEmptyList_WhenDatabaseHasNoBooks()
        {
            var fakeDatabase = new FakeDatabase();
            fakeDatabase.Books.Clear();
            var handler = new GetAllBooksQueryHandler(fakeDatabase);

            var result = await handler.Handle(new GetAllBooksQuery(), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }
    }
}
