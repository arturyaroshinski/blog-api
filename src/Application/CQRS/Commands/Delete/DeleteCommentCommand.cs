using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Delete
{
    public class DeleteCommentCommand : IRequest
    {
        [Required]
        public int Id { get; set; }
    }
    
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCommentCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Comments.FindAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Comment), request.Id);
            }

            _context.Comments.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}