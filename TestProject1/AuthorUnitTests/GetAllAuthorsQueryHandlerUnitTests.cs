using Application.Queries.AuthorQueries.GetAll;
using Infrastructure.Database;

namespace TestProject1.AuthorUnitTests
{
    public class GetAllAuthorsQueryHandlerUnitTests
    {
        private FakeDatabase _fakeDatabase;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
        }

        [Test]
        public async Task GetAllAuthors_ShouldReturnListOfAuthors_WhenDatabaseHasAuthors()
        {
            var fakeDatabase = new FakeDatabase();
            var handler = new GetAllAuthorsQueryHandler(fakeDatabase);

            var result = await handler.Handle(new GetAllAuthorsQuery(), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(fakeDatabase.Authors.Count));
            Assert.That(result[0].Name, Is.EqualTo(fakeDatabase.Authors[0].Name));
        }

        [Test]
        public async Task GetAllAuthors_ShouldReturnEmptyList_WhenDatabaseHasNoAuthors()
        {
            var fakeDatabase = new FakeDatabase();
            fakeDatabase.Authors.Clear();
            var handler = new GetAllAuthorsQueryHandler(fakeDatabase);

            var result = await handler.Handle(new GetAllAuthorsQuery(), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }
    }
}
