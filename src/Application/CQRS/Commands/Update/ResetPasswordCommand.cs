using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Exceptions;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Update
{
    public class ResetPasswordCommand : IRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
    
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuthorizationService _authService;

        public ResetPasswordCommandHandler(
            IApplicationDbContext context,
            IAuthorizationService authService)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var author = _context.Authors.SingleOrDefault(a =>
                a.ResetToken == request.Token &&
                a.ResetTokenExpires > DateTime.UtcNow);

            if (author == null) throw new AppException("Invalid token");
            
            author.PasswordHash = _authService.HashPassword(request.Password);
            author.PasswordReset = DateTime.UtcNow;
            author.ResetToken = null;
            author.ResetTokenExpires = null;
            
            _context.Authors.Update(author);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}