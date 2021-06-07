using System;
using System.Collections.Generic;

namespace Yaroshinski.Blog.Domain.Entities
{
    public class Author
    {
        public int Id { get; set; }
        
        public string PasswordHash { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime BirthDate { get; set; }

        public string About { get; set; }

        public byte[] Avatar { get; set; }
        
        public string VerificationToken { get; set; }
        
        public DateTime? Verified { get; set; }

        public DateTime? ResetTokenExpires { get; set; }
        
        public string ResetToken { get; set; }
        public DateTime? PasswordReset { get; set; }
        
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public Role Role { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
