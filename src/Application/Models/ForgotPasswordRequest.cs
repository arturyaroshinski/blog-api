using System.ComponentModel.DataAnnotations;

namespace Yaroshinski.Blog.Api.Models
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}