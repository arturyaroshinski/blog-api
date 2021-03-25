using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Application.Models;
using Yaroshinski.Blog.Domain.Entities;
using Yaroshinski.Blog.Domain.Exceptions;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Update
{
    public class AuthenticateCommand : IRequest<AuthenticateResponse>
    {
        [Required] [EmailAddress] public string Email { get; set; }

        [Required] public string Password { get; set; }

        public string IpAddress { get; set; }
    }

    public class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand, AuthenticateResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authService;

        public AuthenticateCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IAuthorizationService authService)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<AuthenticateResponse> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            var author = _context.Authors.SingleOrDefault(x => x.Email == request.Email);

            if (author == null ||
                !(author.Verified.HasValue || author.PasswordReset.HasValue) ||
                !_authService.VerifyPassword(request.Password, author.PasswordHash))
                throw new AppException("Email or password is incorrect");

            var jwtToken = _authService.GenerateJwtToken(author);
            var refreshToken = _authService.GenerateRefreshToken(author.Id, request.IpAddress);

            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await _authService.RemoveOldRefreshTokens(author.Id);

            var response = _mapper.Map<AuthenticateResponse>(author);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;
            return response;
        }
    }
}