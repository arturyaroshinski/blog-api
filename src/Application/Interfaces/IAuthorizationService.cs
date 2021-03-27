using System.Threading.Tasks;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.Interfaces
{
    public interface IAuthorizationService
    {
        Task RemoveOldRefreshTokens(int authorId);
        string GenerateJwtToken(Author author);
        RefreshToken GenerateRefreshToken(int authorId, string ipAddress);
        string RandomTokenString();
        Task<bool> OwnsToken(string token);
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}