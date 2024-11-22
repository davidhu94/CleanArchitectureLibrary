using Application.Commands.AuthorCommands.UpdateAuthor;
using Infrastructure.Database;

namespace TestProject1.AuthorUnitTests
{
    public class UpdateAuthorCommandHandlerTests
    {
        private FakeDatabase _fakeDatabase;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
        }

        [Test]
        public async Task UpdateAuthor_ShouldReturnNoContent_WhenAuthorUpdated()
        {
            var command = new UpdateAuthorCommand(1, "Updated Author Name");
            var handler = new UpdateAuthorCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task UpdateAuthor_ShouldReturnTrue_WhenAuthorExists()
        {
            var command = new UpdateAuthorCommand(1, "New Author Name");
            var handler = new UpdateAuthorCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.IsTrue(result);

            var updatedAuthor = _fakeDatabase.Authors.FirstOrDefault(a => a.Id == 1);
            Assert.IsNotNull(updatedAuthor);
            Assert.AreEqual("New Author Name", updatedAuthor.Name);
        }

        [Test]
        public async Task UpdateAuthor_ShouldReturnFalse_WhenAuthorNotFound()
        {
            var command = new UpdateAuthorCommand(999, "Nonexistent Author");
            var handler = new UpdateAuthorCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.IsFalse(result);
        }
    }
}
