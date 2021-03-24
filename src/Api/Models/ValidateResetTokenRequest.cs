using System.ComponentModel.DataAnnotations;

namespace Yaroshinski.Blog.Api.Models
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}