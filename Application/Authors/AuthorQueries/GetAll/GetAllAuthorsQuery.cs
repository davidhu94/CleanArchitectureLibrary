using Application.DTOs.AuthorDTOs;
using Domain.Models;
using MediatR;

namespace Application.Authors.AuthorQueries.GetAll
{
    public record GetAllAuthorsQuery : IRequest<OperationResult<List<AuthorDto>>>
    {
    }
}
