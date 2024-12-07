using Application.Authors.AuthorCommands.AddAuthor;
using Application.Authors.AuthorCommands.DeleteAuthor;
using Application.Authors.AuthorCommands.UpdateAuthor;
using Application.Authors.AuthorQueries.GetAll;
using Application.Authors.AuthorQueries.GetById;
using Application.DTOs.AuthorDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(IMediator mediator, ILogger<AuthorController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            try
            {
                var result = await _mediator.Send(new GetAllAuthorsQuery());

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Failed to retrieve authors: {ErrorMessage}", result.ErrorMessage);
                    return BadRequest(result.ErrorMessage);
                }

                _logger.LogInformation("Successfully retrieved {AuthorCount} authors.", result.Data.Count);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving authors.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching authors.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid author ID provided: {AuthorId}", id);
                return BadRequest("Author ID must be greater than zero.");
            }

            try
            {
                var result = await _mediator.Send(new GetAuthorByIdQuery(id));

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Author retrieval failed: {ErrorMessage}", result.ErrorMessage);
                    return NotFound(result.ErrorMessage);
                }

                _logger.LogInformation("Successfully retrieved author with ID: {AuthorId}.", id);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching author with ID {AuthorId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching the author.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromBody] AddAuthorDto addAuthorDto)
        {
            if (addAuthorDto == null)
            {
                _logger.LogWarning("Attempt to add a null author.");
                return BadRequest("Author data is required.");
            }

            try
            {
                _logger.LogInformation("Adding a new author with name: {AuthorName}", addAuthorDto.Name);

                var command = new AddAuthorCommand(addAuthorDto.Name);
                var result = await _mediator.Send(command);

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Failed to add author: {ErrorMessage}", result.ErrorMessage);
                    return BadRequest(result.ErrorMessage);
                }

                _logger.LogInformation("Successfully added new author with ID: {AuthorId}.", result.Data.Id);
                return CreatedAtAction(nameof(GetAuthorById), new { id = result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the author.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the author.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] UpdateAuthorDto updateAuthorDto)
        {
            if (updateAuthorDto == null)
            {
                _logger.LogWarning("Attempt to update with null author data.");
                return BadRequest("Updated author data is required.");
            }

            if (id != updateAuthorDto.Id)
            {
                _logger.LogWarning("Mismatch between URL ID and request body ID: URL({UrlId}), Body({BodyId}).", id, updateAuthorDto.Id);
                return BadRequest("The ID in the URL must match the author ID in the request body.");
            }

            if (string.IsNullOrWhiteSpace(updateAuthorDto.Name))
            {
                _logger.LogWarning("Attempt to update author with missing name: {AuthorId}", id);
                return BadRequest("Author name is required.");
            }

            try
            {
                _logger.LogInformation("Updating author with ID: {AuthorId}", id);

                var command = new UpdateAuthorCommand(updateAuthorDto.Id, updateAuthorDto.Name);
                var result = await _mediator.Send(command);

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Failed to update author with ID {AuthorId}: {ErrorMessage}", id, result.ErrorMessage);
                    return NotFound(result.ErrorMessage);
                }

                _logger.LogInformation("Successfully updated author with ID: {AuthorId}.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the author with ID: {AuthorId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the author.");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid author ID provided: {AuthorId}", id);
                return BadRequest("Invalid author ID.");
            }

            try
            {
                _logger.LogInformation("Deleting author with ID: {AuthorId}", id);

                var result = await _mediator.Send(new DeleteAuthorCommand(id));

                if (!result.IsSuccessful)
                {
                    _logger.LogWarning("Failed to delete author with ID {AuthorId}: {ErrorMessage}", id, result.ErrorMessage);
                    return NotFound(result.ErrorMessage);
                }

                _logger.LogInformation("Successfully deleted author with ID: {AuthorId}.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the author with ID: {AuthorId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the author.");
            }
        }

    }
}

