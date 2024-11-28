using Application.Commands.AuthorCommands.AddAuthor;
using Application.Commands.AuthorCommands.DeleteAuthor;
using Application.Commands.AuthorCommands.UpdateAuthor;
using Application.DTOs.AuthorDTOs;
using Application.Mappers;
using Application.Queries.AuthorQueries.GetAll;
using Application.Queries.AuthorQueries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : Controller
    {
        public readonly IMediator _mediator;

        public AuthorController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            try
            {
                var authors = await _mediator.Send(new GetAllAuthorsQuery());
                var authorDtos = authors.Select(AuthorMapper.ToDto).ToList();
                return Ok(authorDtos);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Author ID must be greater than zero.");
            }

            var author = await _mediator.Send(new GetAuthorByIdQuery(id));

            if (author == null)
            {
                return NotFound($"Author with ID {id} not found.");
            }
            var authorDto = AuthorMapper.ToDto(author);
            return Ok(authorDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorDto addAuthorDto)
        {
            if (addAuthorDto == null)
            {
                return BadRequest("Author data is required.");
            }

            var author = AuthorMapper.ToModel(addAuthorDto);
            var command = new AddAuthorCommand(author.Name);

            var newAuthorId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetAuthorById), new { id = newAuthorId }, new { id = newAuthorId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] UpdateAuthorDto updateAuthorDto)
        {

            if (updateAuthorDto == null)
            {
                return BadRequest("Updated author data is required.");
            }

            if (id != updateAuthorDto.Id)
            {
                return BadRequest("The ID in the URL must match the author ID in the request body.");
            }

            if (string.IsNullOrWhiteSpace(updateAuthorDto.Name))
            {
                return BadRequest("Author name is required.");
            }

            var author = AuthorMapper.ToModel(updateAuthorDto);
            var command = new UpdateAuthorCommand(author.Id, author.Name);

            var updated = await _mediator.Send(command);

            if (!updated)
            {
                return NotFound($"Author with ID {id} not found.");
            }

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid author ID.");
            }

            var result = await _mediator.Send(new DeleteAuthorCommand(id));

            if (!result)
            {
                return NotFound($"Author with ID {id} was not found.");
            }

            return NoContent();
        }

    }
}
