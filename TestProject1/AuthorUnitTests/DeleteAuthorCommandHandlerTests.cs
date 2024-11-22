using Application.Commands.AuthorCommands.DeleteAuthor;
using Infrastructure.Database;

namespace TestProject1.AuthorUnitTests
{
    public class DeleteAuthorCommandHandlerTests
    {
        private FakeDatabase _fakeDatabase;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
        }

        [Test]
        public async Task DeleteAuthor_ShouldReturnTrue_WhenAuthorExists()
        {
            var command = new DeleteAuthorCommand(1);
            var handler = new DeleteAuthorCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.IsTrue(result);

            Assert.IsFalse(_fakeDatabase.Authors.Any(a => a.Id == 1));
        }

        [Test]
        public async Task DeleteAuthor_ShouldReturnFalse_WhenAuthorDoesNotExist()
        {
            var command = new DeleteAuthorCommand(999);
            var handler = new DeleteAuthorCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.IsFalse(result);

            Assert.AreEqual(5, _fakeDatabase.Authors.Count);
        }
    }
}
