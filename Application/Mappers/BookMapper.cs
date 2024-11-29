using Application.Commands.BookCommands.UpdateBook;
using Application.DTOs.BookDTOs;
using Domain.Models;

namespace Application.Mappers
{
    public static class BookMapper
    {
        public static BookDto ToDto(Book book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                AuthorId = book.AuthorId
            };
        }

        public static Book ToModel(AddBookDto addBookDto)
        {
            return new Book
            {
                AuthorId = addBookDto.AuthorId,
                Title = addBookDto.Title,
                Description = addBookDto.Description
            };

        }

        public static Book ToModel(UpdateBookDto updateBookDto)
        {
            return new Book
            {
                Id = updateBookDto.Id,
                Title = updateBookDto.Title,
                Description = updateBookDto.Description,
                AuthorId = updateBookDto.AuthorId
            };
        }

        public static UpdateBookCommand ToUpdateCommand(UpdateBookDto updateBookDto)
        {
            return new UpdateBookCommand(
                updateBookDto.Id,
                updateBookDto.Title,
                updateBookDto.Description,
                updateBookDto.AuthorId
            );
        }
    }
}
