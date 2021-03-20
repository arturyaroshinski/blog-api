using System;

namespace Yaroshinski.Blog.Application.DTO
{
    public class AuthorDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string About { get; set; }

        public byte[] Avatar { get; set; }
    }
}
