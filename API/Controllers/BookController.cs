using Application.Commands.BookCommands.AddBook;
using Application.Commands.BookCommands.DeleteBook;
using Application.Commands.BookCommands.UpdateBook;
using Application.DTOs.BookDTOs;
using Application.Mappers;
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
            try
            {
                var books = await _mediator.Send(new GetAllBooksQuery());
                var bookDtos = books.Select(BookMapper.ToDto).ToList();
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
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
            var bookDto = BookMapper.ToDto(book);
            return Ok(bookDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] AddBookDto addBookDto)
        {
            if (addBookDto == null)
            {
                return BadRequest("Book data is required.");
            }

            //var command = BookMapper.ToModel(addBookDto);
            var command = new AddBookCommand(addBookDto.Title, addBookDto.Description, addBookDto.AuthorId);
            var newBookId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetBookById), new { id = newBookId }, new { id = newBookId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (updateBookDto == null)
            {
                return BadRequest("Updated book data is required.");
            }

            if (id != updateBookDto.Id)
            {
                return BadRequest("The Id in the URL must match the book Id in the body.");
            }

            if (string.IsNullOrWhiteSpace(updateBookDto.Title))
            {
                return BadRequest("Book title is required.");
            }

            if (updateBookDto.AuthorId <= 0)
            {
                return BadRequest("A valid Author ID is required.");
            }

            var command = BookMapper.ToUpdateCommand(updateBookDto);

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
