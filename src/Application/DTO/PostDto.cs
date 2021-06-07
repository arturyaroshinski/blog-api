using System;

namespace Yaroshinski.Blog.Application.DTO
{
    public class PostDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public int AuthorId { get; set; }
    }
}
