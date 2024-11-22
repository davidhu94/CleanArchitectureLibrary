using Application.Commands.BookCommands.UpdateBook;
using Infrastructure.Database;

namespace TestProject1.BookUnitTests
{
    public class UpdateBookCommandHandlerTests
    {
        private FakeDatabase _fakeDatabase;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
        }

        [Test]
        public async Task UpdateBook_ShouldReturnTrue_WhenBookUpdated()
        {
            var command = new UpdateBookCommand(1, "Updated Book Title", "Updated Description", 1);
            var handler = new UpdateBookCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task UpdateBook_ShouldReturnTrue_WhenBookExists()
        {
            var command = new UpdateBookCommand(1, "New Book Title", "New Description", 1);
            var handler = new UpdateBookCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.IsTrue(result);

            var updatedBook = _fakeDatabase.Books.FirstOrDefault(b => b.Id == 1);
            Assert.IsNotNull(updatedBook);
            Assert.AreEqual("New Book Title", updatedBook.Title);
            Assert.AreEqual("New Description", updatedBook.Description);
        }

        [Test]
        public async Task UpdateBook_ShouldReturnFalse_WhenBookNotFound()
        {
            var command = new UpdateBookCommand(999, "Nonexistent Book", "Nonexistent Description", 1);
            var handler = new UpdateBookCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.IsFalse(result);
        }
    }
}
