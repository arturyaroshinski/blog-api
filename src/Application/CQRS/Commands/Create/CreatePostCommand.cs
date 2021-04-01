using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Create
{
    public class CreatePostCommand : IRequest<int>
    {
        [Required] 
        public string Title { get; set; }

        [Required] 
        public string Text { get; set; }

        [JsonIgnore] 
        public int AuthorId { get; set; }
    }

    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreatePostCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            var newPost = new Post
            {
                AuthorId = request.AuthorId,
                Title = request.Title,
                Text = request.Text,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Posts.AddAsync(newPost, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            
            return newPost.Id;
        }
    }
}