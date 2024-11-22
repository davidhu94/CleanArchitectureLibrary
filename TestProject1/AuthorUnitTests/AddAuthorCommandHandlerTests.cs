
using Application.Commands.AuthorCommands.AddAuthor;
using Infrastructure.Database;

namespace TestProject1.AuthorUnitTests
{
    public class AddAuthorCommandHandlerTests
    {
        private FakeDatabase _fakeDatabase;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
        }

        [Test]
        public async Task AddAuthor_ShouldReturnNewAuthorId_WhenAuthorIsAddedSuccessfully()
        {
            var command = new AddAuthorCommand("David");
            var handler = new AddAuthorCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.AreEqual(6, result);
            Assert.AreEqual(6, _fakeDatabase.Authors.Count);
            Assert.AreEqual("David", _fakeDatabase.Authors[5].Name);
        }

        [Test]
        public void AddAuthor_ShouldThrowArgumentException_WhenNameIsNullOrWhitespace()
        {

            var command = new AddAuthorCommand(null);
            var handler = new AddAuthorCommandHandler(_fakeDatabase);

            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, default));
        }

        [Test]
        public void AddAuthor_ShouldThrowArgumentException_WhenNameIsWhitespace()
        {
            var command = new AddAuthorCommand(" ");
            var handler = new AddAuthorCommandHandler(_fakeDatabase);

            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, default));
        }
    }
}
