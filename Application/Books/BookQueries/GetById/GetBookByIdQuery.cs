using Application.DTOs.BookDTOs;
using Domain.Models;
using MediatR;

namespace Application.Books.BookQueries.GetById
{
    public record GetBookByIdQuery(int Id) : IRequest<OperationResult<BookDto>>;
}
