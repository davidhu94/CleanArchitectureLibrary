using Application.DTOs.AuthorDTOs;
using Domain.Models;
using MediatR;

namespace Application.Authors.AuthorQueries.GetById
{
    public record GetAuthorByIdQuery(int Id) : IRequest<OperationResult<AuthorDto>>;
}
