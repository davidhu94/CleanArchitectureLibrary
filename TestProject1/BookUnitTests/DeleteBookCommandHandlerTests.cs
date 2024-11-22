using Application.Commands.BookCommands.DeleteBook;
using Infrastructure.Database;

namespace TestProject1.BookUnitTests
{
    public class DeleteBookCommandHandlerTests
    {
        private FakeDatabase _fakeDatabase;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
        }

        [Test]
        public async Task DeleteBook_ShouldReturnTrue_WhenBookExists()
        {
            var bookIdToDelete = 1;
            var command = new DeleteBookCommand(bookIdToDelete);

            var handler = new DeleteBookCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.That(result, Is.True);
            Assert.That(_fakeDatabase.Books.All(b => b.Id != bookIdToDelete), Is.True);
        }

        [Test]
        public async Task DeleteBook_ShouldReturnFalse_WhenBookDoesNotExist()
        {
            var bookIdToDelete = 999;
            var command = new DeleteBookCommand(bookIdToDelete);

            var handler = new DeleteBookCommandHandler(_fakeDatabase);

            var result = await handler.Handle(command, default);

            Assert.That(result, Is.False);  
        }

        [Test]
        public void DeleteBook_ShouldThrowArgumentException_WhenInvalidId()
        {
            var commandWithInvalidId = new DeleteBookCommand(0);  

            var handler = new DeleteBookCommandHandler(_fakeDatabase);

            Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(commandWithInvalidId, default));
        }
    }
}
