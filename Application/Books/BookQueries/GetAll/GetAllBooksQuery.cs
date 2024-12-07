using Application.DTOs.BookDTOs;
using Domain.Models;
using MediatR;

namespace Application.Books.BookQueries.GetAll
{
    public record GetAllBooksQuery() : IRequest<OperationResult<List<BookDto>>>;

}
