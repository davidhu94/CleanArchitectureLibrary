﻿namespace Application.DTOs.BookDTOs
{
    public class AddBookDto
    {

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int AuthorId { get; set; }
    }
}
