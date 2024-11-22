
using Application.Commands.AuthorCommands.UpdateAuthor;
using Application.Commands.BookCommands.AddBook;
using Application.Commands.BookCommands.DeleteBook;
using Application.Commands.BookCommands.UpdateBook;
using Application.Queries.BookQueries.GetAll;
using Application.Queries.BookQueries.GetById;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        public readonly IMediator _mediator;
        public BookController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _mediator.Send(new GetAllBooksQuery());
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Book ID must be greater than zero.");
            }

            var book = await _mediator.Send(new GetBookByIdQuery(id));
            if (book == null)
            {
                return NotFound($"The book with ID {id} was not found.");
            }

            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] AddBookCommand command)
        {
            if (command == null)
            {
                return BadRequest("Book data is required.");
            }

            var newBookId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetBookById), new { id = newBookId }, new { id = newBookId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book updatedBook)
        {
            if (updatedBook == null)
            {
                return BadRequest("Updated book data is required.");
            }

            if (id != updatedBook.Id)
            {
                return BadRequest("The Id in the URL must match the book Id in the body.");
            }

            if (string.IsNullOrWhiteSpace(updatedBook.Title))
            {
                return BadRequest("Book title is required.");
            }

            if (updatedBook.AuthorId <= 0)
            {
                return BadRequest("A valid Author ID is required.");
            }

            var command = new UpdateBookCommand(updatedBook.Id, updatedBook.Title, updatedBook.Description, updatedBook.AuthorId);

            var result = await _mediator.Send(command);
            if (!result)
            {
                return NotFound($"Book with ID {id} was not found.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Book ID must be greater than zero.");
            }

            var book = await _mediator.Send(new GetBookByIdQuery(id));
            if (book == null)
            {
                return NotFound($"Book with ID {id} was not found.");
            }

            await _mediator.Send(new DeleteBookCommand(id));
            return NoContent();
        }
    }
}
