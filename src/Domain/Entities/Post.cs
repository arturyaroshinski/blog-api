using System;
using System.Collections.Generic;

namespace Yaroshinski.Blog.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }

        public ICollection<PostTag> PostTags { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
