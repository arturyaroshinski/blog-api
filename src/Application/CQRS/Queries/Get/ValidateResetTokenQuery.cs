using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Queries.Get
{
    public class ValidateResetTokenQuery : IRequest
    {
        [Required]
        public string Token { get; set; }
    }

    public class ValidateResetTokenQueryHandler : IRequestHandler<ValidateResetTokenQuery>
    {
        private readonly IApplicationDbContext _context;

        public ValidateResetTokenQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        
        public Task<Unit> Handle(ValidateResetTokenQuery request, CancellationToken cancellationToken)
        {
            var author = _context.Authors.SingleOrDefault(a =>
                a.ResetToken == request.Token &&
                a.ResetTokenExpires > DateTime.UtcNow);

            if (author == null) throw new NotFoundException(nameof(Author), request.Token);

            return Task.FromResult(Unit.Value);
        }
    }
}