using System.Threading.Tasks;
using Yaroshinski.Blog.Application.CQRS.Commands.Create;
using Yaroshinski.Blog.Application.CQRS.Commands.Update;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.Interfaces
{
    public interface IAuthorizationService
    {
        Task RemoveOldRefreshTokens(int authorId);
        string GenerateJwtToken(Author author);
        RefreshToken GenerateRefreshToken(int authorId, string ipAddress);
        Task Register(CreateAuthorCommand model, string origin);
        Task ForgotPassword(ForgotPasswordCommand command, string origin);
        Task ValidateResetToken(string token);
        string RandomTokenString();
        Task<bool> OwnsToken(string token);
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}