using BC = BCrypt.Net.BCrypt;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Yaroshinski.Blog.Application.Configuration;
using Yaroshinski.Blog.Application.CQRS.Commands.Create;
using Yaroshinski.Blog.Application.CQRS.Commands.Delete;
using Yaroshinski.Blog.Application.CQRS.Commands.Update;
using Yaroshinski.Blog.Application.CQRS.Queries.Get;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;
using Yaroshinski.Blog.Domain.Exceptions;

namespace Yaroshinski.Blog.Infrastructure.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;
        private readonly IMediator _mediator;

        public AuthorizationService(
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IEmailService emailService,
            IMediator mediator
        )
        {
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _mediator = mediator;
        }

        private async Task<RefreshTokenDto> GetRefreshToken(string token)
        {
            var query = new GetAuthorByPredicateQuery
            {
                Predicate = (a) => a.RefreshTokens.Any(t => t.Token == token)
            };

            var author = await _mediator.Send(query);
            
            var refreshToken = await _mediator.Send(new GetRefreshTokenQuery {Token = token});
            // extract is active method
            if (!(refreshToken.Revoked == null && !refreshToken.IsExpired))
                throw new AppException("Invalid token");
            
            return refreshToken;
        }

        public string GenerateJwtToken(Author author)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {new Claim("id", author.Id.ToString())}),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(int authorId, string ipAddress)
        {
            return new RefreshToken
            {
                AuthorId = authorId,
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        public async Task RemoveOldRefreshTokens(int authorId)
        {
            var oldTokens = await _mediator.Send(new GetOldTokensQuery { AuthorId = authorId});
            await _mediator.Send(new DeleteTokensCommand { Tokens = oldTokens});
        }

        public string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        public async Task<bool> OwnsToken(string token)
        {
            var query = new GetAuthorByPredicateQuery
            {
                Predicate = a => a.RefreshTokens.Any(t => t.Token == token)
            };
            
            try
            {
                await _mediator.Send(query);
            }
            catch (NotFoundException)
            {
                return false;
            }

            return true;
        }

        public string HashPassword(string password)
        {
            return BC.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BC.Verify(password, passwordHash);
        }

        private void SendVerificationEmail(Author account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else
            {
                message =
                    $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
            );
        }

        private void SendAlreadyRegisteredEmail(string email, string origin)
        {
            var message =
                !string.IsNullOrEmpty(origin)
                    ? $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>"
                    : "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

            _emailService.Send(
                to: email,
                subject: "Sign-up Verification API - Email Already Registered",
                html: $@"<h4>Email Already Registered</h4>
                         <p>Your email <strong>{email}</strong> is already registered.</p>
                         {message}"
            );
        }

        private void SendPasswordResetEmail(AuthorDto author, string origin)
        {
            string message;
            if (string.IsNullOrEmpty(origin))
            {
                message =
                    $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{author.ResetToken}</code></p>";
            }
            else
            {
                var resetUrl = $"{origin}/account/reset-password?token={author.ResetToken}";
                message =
                    $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }

            _emailService.Send(
                to: author.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }
    }
}