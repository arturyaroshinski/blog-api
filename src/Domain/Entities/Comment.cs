using System;

namespace Yaroshinski.Blog.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public int AuthorId { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public Post Post { get; set; }

        public Author Author { get; set; }
    }
}
