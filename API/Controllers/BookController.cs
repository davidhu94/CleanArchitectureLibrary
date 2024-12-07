using Application.Books.BookCommands.AddBook;
using Application.Books.BookCommands.DeleteBook;
using Application.Books.BookQueries.GetAll;
using Application.Books.BookQueries.GetById;
using Application.DTOs.BookDTOs;
using Application.Mappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        public readonly IMediator _mediator;
        private readonly ILogger<BookController> _logger;
        public BookController(IMediator mediator, ILogger<BookController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var result = await _mediator.Send(new GetAllBooksQuery());

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("An error occurred: {ErrorMessage}", result.ErrorMessage);
                    return StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching books.");
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

            try
            {
                var result = await _mediator.Send(new GetBookByIdQuery(id));

                if (!result.IsSuccessful)
                {
                    return NotFound(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the book with ID {BookId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching the book.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] AddBookDto addBookDto)
        {
            if (addBookDto == null || string.IsNullOrWhiteSpace(addBookDto.Title))
            {
                _logger.LogWarning("AddBook endpoint called with invalid data.");
                return BadRequest("Book data and title are required.");
            }

            try
            {
                var command = new AddBookCommand(addBookDto.Title, addBookDto.Description, addBookDto.AuthorId);
                var result = await _mediator.Send(command);

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Failed to add book: {ErrorMessage}", result.ErrorMessage);
                    return BadRequest(result.ErrorMessage);
                }

                _logger.LogInformation("Book created successfully with ID: {BookId}", result.Data.Id);
                return CreatedAtAction(nameof(GetBookById), new { id = result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the book.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the book.");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (updateBookDto == null || id <= 0 || id != updateBookDto.Id 
                || string.IsNullOrWhiteSpace(updateBookDto.Title) 
                || string.IsNullOrWhiteSpace(updateBookDto.Description) 
                || updateBookDto.AuthorId <= 0)
            {
                _logger.LogWarning("Invalid data for book update.");
                return BadRequest("Invalid book data.");
            }

            try
            {
                var command = BookMapper.ToUpdateCommand(updateBookDto);
                var result = await _mediator.Send(command);

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Failed to update book with ID {BookId}: {ErrorMessage}", id, result.ErrorMessage);
                    return NotFound(result.ErrorMessage);
                }

                _logger.LogInformation("Successfully updated book with ID: {BookId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the book with ID {BookId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the book.");
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("DeleteBook endpoint called with invalid ID: {Id}", id);
                return BadRequest("Book ID must be greater than zero.");
            }

            try
            {
                var command = new DeleteBookCommand(id);
                var result = await _mediator.Send(command);

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Attempted to delete book with ID: {Id}, but it was not found.", id);
                    return NotFound(result.ErrorMessage);
                }

                _logger.LogInformation("Book with ID: {Id} successfully deleted.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the book with ID {BookId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the book.");
            }
        }

    }
}
