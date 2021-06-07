using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Update
{
    public class ForgotPasswordCommand : IRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
    
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuthorizationService _authService;

        public ForgotPasswordCommandHandler(IApplicationDbContext context, IAuthorizationService authService)
        {
            _context = context;
            _authService = authService;
        }
        
        public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var entity = _context.Authors.SingleOrDefault(x => x.Email == request.Email);

            if (entity == null) throw new NotFoundException(nameof(Author), request.Email);

            entity.ResetToken = _authService.RandomTokenString();
            entity.ResetTokenExpires = DateTime.UtcNow.AddDays(1);
            
            _context.Authors.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            // TODO: send email
            return Unit.Value;
        }
    }
}