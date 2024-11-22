using Application;
using Domain.Models;
using Infrastructure.Database;

namespace TestProject
{
    public class BookUnitTest
    {
        // SKRIVA UNIT TESTER PÅ:
        // CRUD I APPLICATION (BOOKMETHODS)
        // INFRASTRUCTURE (MOQ)
        // API 

        // SKRIVA INTEGRATION TEST PÅ:
        // API -> APPLICATION

        // INTEGRATION TEST EJ VIKTIGT.
        // 
        // VIKTIGT:
        // CLEAN ARCHITECTURE SOM SÄTT ATT PROGRAMMERA/BYGGA PROJEKT
        // SKRIVA TESTER
        // SEPARATION OF CONCERNS (SOP)

        private FakeDatabase _fakeDatabase;

        [SetUp]
        public void Setup()
        {
            _fakeDatabase = new FakeDatabase();
        }

        

        //[Test]
        //public void When_Method_AddNewBook_IsCalled_Then_BookAddedToList()
        //{
        //    // Arrange
        //    Book bookToTest = new Book(1, "NemoBook", "Book of Nemo");   
        //    // Act
        //    //Book bookCreated = BookMethods.AddNewBook();

        //    //// Assert
        //    //Assert.That(bookToTest, Is.Not.Null);
        //    //Assert.That(bookCreated.Description, Is.EqualTo(bookToTest.Description));
        //}

        //[Test] 
        //public void When_Method_GetAllBooks_IsCalled_Then_ReturnAllBooks()
        //{
        //    List<Book> books = BookMethods.GetAllBooks();
        //}
    }
}