using System;
using System.Collections.Generic;

namespace Yaroshinski.Blog.Domain.Entities
{
    public class Author
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string About { get; set; }

        public byte[] Avatar { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
