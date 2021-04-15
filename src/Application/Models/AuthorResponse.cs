using System;

namespace Yaroshinski.Blog.Application.Models
{
    public class AuthorResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime BirthDate { get; set; }

        public string About { get; set; }

        public byte[] Avatar { get; set; }

        public string VerificationToken { get; set; }

        public DateTime Created { get; set; }

        public string Role { get; set; }
    }
}
