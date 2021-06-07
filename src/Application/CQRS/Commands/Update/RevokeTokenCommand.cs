using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Update
{
    public class RevokeTokenCommand : IRequest
    {
        public string Token { get; set; }

        [JsonIgnore]
        public string IpAddress { get; set; }
    }
    
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
    {
        private readonly IApplicationDbContext _context;

        public RevokeTokenCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = _context.RefreshTokens.SingleOrDefault(t => t.Token.Equals(request.Token));

            if (refreshToken == null) throw new NotFoundException(nameof(RefreshToken), request.Token);
            
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = request.IpAddress;

            await _context.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}