using System.Collections.Generic;
using Yaroshinski.Blog.Api.Models;
using Yaroshinski.Blog.Application.DTO;

namespace Yaroshinski.Blog.Application.Interfaces
{
    public interface IAuthorService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        void Register(RegisterRequest model, string origin);
        void VerifyEmail(string token);
        void ForgotPassword(ForgotPasswordRequest model, string origin);
        void ValidateResetToken(ValidateResetTokenRequest model);

        void ResetPassword(ResetPasswordRequest model);
        List<AuthorDto> GetAll();
        AuthorResponse GetById(int id);
        AuthorResponse Create(CreateAuthorRequest model);
        AuthorResponse Update(int id, UpdateAuthorRequest model);
        void Delete(int id);
    }
}