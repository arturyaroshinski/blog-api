using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Application.Models;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Update
{
    public class RefreshTokenCommand : IRequest<AuthenticateResponse>
    {
        public string Token { get; set; }
        public string IpAddress { get; set; }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticateResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuthorizationService _authService;
        private readonly IMapper _mapper;

        public RefreshTokenCommandHandler(
            IApplicationDbContext context,
            IAuthorizationService authService,
            IMapper mapper)
        {
            _context = context;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<AuthenticateResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var oldToken = _context
                .RefreshTokens
                .SingleOrDefault(t => t.Token.Equals(request.Token));

            if (oldToken == null) throw new NotFoundException(nameof(RefreshToken), request.Token);

            var newToken = _authService.GenerateRefreshToken(oldToken.AuthorId, request.IpAddress);

            oldToken.Revoked = DateTime.UtcNow;
            oldToken.RevokedByIp = request.IpAddress;
            oldToken.ReplacedByToken = newToken.Token;
            
            await _context.RefreshTokens.AddAsync(newToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await _authService.RemoveOldRefreshTokens(oldToken.AuthorId);

            var author = await _context.Authors.FindAsync(oldToken.AuthorId);
            var newJwtToken = _authService.GenerateJwtToken(author);

            var response = _mapper.Map<AuthenticateResponse>(author);
            response.JwtToken = newJwtToken;
            response.RefreshToken = newToken.Token;

            return response;
        }
    }
}