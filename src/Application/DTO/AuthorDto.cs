using System;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.DTO
{
    public class AuthorDto
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
        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
        // public bool OwnsToken(string token) 
        // {
        //     return this.RefreshTokens?.Find(x => x.Token == token) != null;
        // }
    }
}
