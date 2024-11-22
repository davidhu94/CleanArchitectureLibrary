using Application.Queries.AuthorQueries.GetById;
using Infrastructure.Database;

namespace TestProject1.AuthorUnitTests
{
    public class GetAuthorByIdQueryHandlerUnitTest
    {
        private FakeDatabase _fakeDatabase;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
        }

        [Test]
        public async Task GetAuthorById_ShouldReturnAuthor_WhenAuthorExists()
        {
            var fakeDatabase = new FakeDatabase();
            var existingAuthorId = 1;
            var handler = new GetAuthorByIdQueryHandler(fakeDatabase);

            var result = await handler.Handle(new GetAuthorByIdQuery(existingAuthorId), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Id, Is.EqualTo(existingAuthorId));
            Assert.That(result?.Name, Is.EqualTo(fakeDatabase.Authors.First(a => a.Id == existingAuthorId).Name));
        }

        [Test]
        public async Task GetAuthorById_ShouldReturnNull_WhenAuthorDoesNotExist()
        {
            var fakeDatabase = new FakeDatabase();
            var nonExistingAuthorId = 999;
            var handler = new GetAuthorByIdQueryHandler(fakeDatabase);

            var result = await handler.Handle(new GetAuthorByIdQuery(nonExistingAuthorId), CancellationToken.None);

            Assert.That(result, Is.Null);
        }
    }
}
