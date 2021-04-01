using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Create
{
    public class CreateCommentCommand : IRequest<int>
    {
        [Required]
        public int PostId { get; set; }
        
        [Required]
        public string Text { get; set; }

        [JsonIgnore]
        public int AuthorId { get; set; }
    }
    
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateCommentCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public Task<int> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = new Comment
            {
                Text = request.Text,
                PostId = request.PostId,
                AuthorId = request.AuthorId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            _context.SaveChangesAsync(cancellationToken);

            return Task.FromResult(comment.Id);
        }
    }
}