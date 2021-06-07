using System;

namespace Yaroshinski.Blog.Application.DTO
{
    public class CommentDto
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public int AuthorId { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
