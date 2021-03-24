using System;

namespace Yaroshinski.Blog.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public int AuthorId { get; set; }

        public string Token { get; set; }
        
        public DateTime Expires { get; set; }
 
        public DateTime Created { get; set; }
        
        public string CreatedByIp { get; set; }
        
        public DateTime? Revoked { get; set; }
        
        public string RevokedByIp { get; set; }
        
        public string ReplacedByToken { get; set; }
        
        public Author Author { get; set; }
    }
}