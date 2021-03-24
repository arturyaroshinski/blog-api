using System.ComponentModel.DataAnnotations;

namespace Yaroshinski.Blog.Api.Models
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}