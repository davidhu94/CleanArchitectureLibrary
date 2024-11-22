using Application.Commands.AuthorCommands.AddAuthor;
using Application.Commands.AuthorCommands.DeleteAuthor;
using Application.Commands.AuthorCommands.UpdateAuthor;
using Application.Queries.AuthorQueries.GetAll;
using Application.Queries.AuthorQueries.GetById;
using Domain.Models;
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
            var authors = await _mediator.Send(new GetAllAuthorsQuery());
            return Ok(authors);
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

            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorCommand command)
        {
            if (command == null)
            {
                return BadRequest("Author data is required.");
            }

            var newAuthorId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetAuthorById), new { id = newAuthorId }, new { id = newAuthorId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] Author updatedAuthor)
        {
            if (updatedAuthor == null)
            {
                return BadRequest("Updated author data is required.");
            }

            if (id != updatedAuthor.Id)
            {
                return BadRequest("The ID in the URL must match the author ID in the request body.");
            }

            if (string.IsNullOrWhiteSpace(updatedAuthor.Name))
            {
                return BadRequest("Author name is required.");
            }

            var command = new UpdateAuthorCommand(updatedAuthor.Id, updatedAuthor.Name);
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
