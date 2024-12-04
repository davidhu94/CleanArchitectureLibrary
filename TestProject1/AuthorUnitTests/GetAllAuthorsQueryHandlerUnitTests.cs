using Application.Interfaces.RepositoryInterfaces;
using Application.Queries.AuthorQueries.GetAll;
using Domain.Models;
using Moq;

namespace TestProject1.AuthorUnitTests
{
    public class GetAllAuthorsQueryHandlerUnitTests
    {
        private Mock<IAuthorRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAuthorRepository>();
        }

        //[Test]
        //public async Task GetAllAuthors_ShouldReturnListOfAuthors_WhenDatabaseHasAuthors()
        //{
        //    var authorsList = new List<Author>
        //    {
        //        new Author { Id = 1, Name = "Author 1" },
        //        new Author { Id = 2, Name = "Author 2" }
        //    };

        //    _repositoryMock.Setup(r => r.GetAllAsync())
        //        .ReturnsAsync(authorsList);

        //    var handler = new GetAllAuthorsQueryHandler(_repositoryMock.Object);

        //    var result = await handler.Handle(new GetAllAuthorsQuery(), CancellationToken.None);

        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.Count, Is.EqualTo(authorsList.Count));
        //    Assert.That(result[0].Name, Is.EqualTo(authorsList[0].Name));
        //}

        //[Test]
        //public async Task GetAllAuthors_ShouldReturnEmptyList_WhenDatabaseHasNoAuthors()
        //{
        //    _repositoryMock.Setup(r => r.GetAllAsync())
        //        .ReturnsAsync(new List<Author>());

        //    var handler = new GetAllAuthorsQueryHandler(_repositoryMock.Object);

        //    var result = await handler.Handle(new GetAllAuthorsQuery(), CancellationToken.None);

        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.Count, Is.EqualTo(0));
        //}
    }
}
