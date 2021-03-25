using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Update
{
    public class VerifyEmailCommand : IRequest
    {
        public string Token { get; set; }
    }
    
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand>
    {
        private readonly IApplicationDbContext _context;

        public VerifyEmailCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Unit> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var entity = _context.Authors.SingleOrDefault(x => x.VerificationToken == request.Token);

            if (entity == null) throw new NotFoundException(nameof(Author), request.Token);
            
            entity.Verified = DateTime.UtcNow;
            entity.VerificationToken = null;

            _context.Authors.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}