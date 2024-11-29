using Application.DTOs.AuthorDTOs;
using Domain.Models;

namespace Application.Mappers
{
    public static class AuthorMapper
    {
        public static AuthorDto ToDto(Author author)
        {
            return new AuthorDto
            {
                Id = author.Id,
                Name = author.Name
            };
        }

        public static Author ToModel(AddAuthorDto addAuthorDto)
        {
            return new Author
            {
                Name = addAuthorDto.Name
            };
        }

        public static Author ToModel(UpdateAuthorDto updateAuthorDto)
        {
            return new Author
            {
                Id = updateAuthorDto.Id,
                Name = updateAuthorDto.Name
            };
        }
    }
}
