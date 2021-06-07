using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Interfaces;

namespace Yaroshinski.Blog.Application.CQRS.Queries.Get
{
    public class GetPostsQuery : IRequest<List<PostDto>>
    {
        // TODO: add page size and offset
    }

    public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, List<PostDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPostsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<List<PostDto>> Handle(GetPostsQuery request,
            CancellationToken cancellationToken)
        {
            var posts = _context.Posts
                .ProjectTo<PostDto>(_mapper.ConfigurationProvider)
                .ToList();

            return Task.FromResult(posts);
        }
    }
}