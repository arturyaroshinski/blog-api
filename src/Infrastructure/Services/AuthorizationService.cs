using BC = BCrypt.Net.BCrypt;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Yaroshinski.Blog.Application.Configuration;
using Yaroshinski.Blog.Application.CQRS.Commands.Delete;
using Yaroshinski.Blog.Application.CQRS.Queries.Get;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Infrastructure.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly AppSettings _appSettings;
        private readonly IMediator _mediator;

        public AuthorizationService(IOptions<AppSettings> appSettings, IMediator mediator)
        {
            _appSettings = appSettings.Value;
            _mediator = mediator;
        }

        public string GenerateJwtToken(Author author)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", author.Id.ToString()) }),
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
            var oldTokens = await _mediator.Send(new GetOldTokensQuery { AuthorId = authorId });
            await _mediator.Send(new DeleteTokensCommand { Tokens = oldTokens });
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
    }
}