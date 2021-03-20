using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Application.Models;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Create
{
    public abstract class CreatePostCommand : IRequest<Response<int>>
    {
        public PostDto Model { get; set; }
    }

    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Response<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreatePostCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response<int>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            var newPost = _mapper.Map<Post>(request.Model);
            try
            {
                await _context.Posts.AddAsync(newPost, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return Response.Fail(ex.Message, -1);
            }

            return Response.Ok("Post was successfully created", newPost.Id);
        }
    }
}