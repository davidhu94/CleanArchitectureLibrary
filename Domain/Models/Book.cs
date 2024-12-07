using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AuthorId must be greater than 0.")]
        public int AuthorId { get; set; }

        public Book(int id, string title, string description, int authorId)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id must be greater than zero.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            }

            if (authorId <= 0)
            {
                throw new ArgumentException("AuthorId must be greater than zero.", nameof(authorId));
            }

            Id = id;
            Title = title;
            Description = description;
            AuthorId = authorId;
        }

        public Book() { }
    }
}
