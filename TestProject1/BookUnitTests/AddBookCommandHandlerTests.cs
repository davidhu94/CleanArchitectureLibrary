using Application.Commands.BookCommands.AddBook;
using Infrastructure.Database;

namespace TestProject1.BookUnitTests
{
    public class AddBookCommandHandlerTests
    {
        private FakeDatabase _fakeDatabase;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
        }

        [Test]
        public async Task AddBook_ShouldReturnNewBookId_WhenValidData()
        {
            var command = new AddBookCommand("New Book Title", "This is a book description.", 1);

            var handler = new AddBookCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.That(result, Is.EqualTo(6));
            Assert.That(_fakeDatabase.Books.Any(b => b.Id == result), Is.True);
        }

        [Test]
        public void AddBook_ShouldThrowArgumentException_WhenTitleOrDescriptionIsMissing()
        {
            var commandWithEmptyTitle = new AddBookCommand("", "This is a description.", 1);
            var commandWithEmptyDescription = new AddBookCommand("Title", "", 1);
            var commandWithNullTitle = new AddBookCommand(null, "This is a description.", 1);
            var commandWithNullDescription = new AddBookCommand("Title", null, 1);

            var handler = new AddBookCommandHandler(_fakeDatabase);

            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(commandWithEmptyTitle, default));
            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(commandWithEmptyDescription, default));
            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(commandWithNullTitle, default));
            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(commandWithNullDescription, default));
        }
    }
}
