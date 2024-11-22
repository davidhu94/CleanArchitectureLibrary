using Application.Commands.AuthorCommands.AddAuthor;
using Infrastructure.Database;
using MediatR;
using Moq;

namespace TestProject
{
    public class AuthorUnitTest
    {
        private FakeDatabase _fakeDatabase;
        private Mock<IMediator> _mediatorMock;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
            _mediatorMock = new Mock<IMediator>();
        }

        [Test]
        public async Task AddAuthor_ShouldReturnNewAuthorId()
        {
            var command = new AddAuthorCommand("David");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AddAuthorCommand>(), default))
                .ReturnsAsync(6);

            var handler = new AddAuthorCommandHandler(_fakeDatabase);
            var result = await handler.Handle(command, default);

            Assert.That(result, Is.EqualTo(6));
            
        }
    }
}
