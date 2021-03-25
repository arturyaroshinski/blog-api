using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Application.Models;

namespace Yaroshinski.Blog.Application.CQRS.Queries.Get
{
    public class GetPostByIdQuery : IRequest<Response<PostDto>>
    {
        public int Id { get; set; }
    }

    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, Response<PostDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPostByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public Task<Response<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var Post = _context.Posts.Find(request.Id);

            if (Post == null)
            {
                throw new NotFoundException(nameof(Post), request.Id);
            }

            var PostDto = _mapper.Map<PostDto>(Post);

            return Task.FromResult(Response.Ok("Post was found successful", PostDto));
        }
    }
}
