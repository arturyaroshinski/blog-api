using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Interfaces;

namespace Yaroshinski.Blog.Application.CQRS.Queries.Get
{
    public class GetRefreshTokenQuery : IRequest<RefreshTokenDto>
    {
        public string Token { get; set; }
    }

    public class GetRefreshTokenQueryHandler : IRequestHandler<GetRefreshTokenQuery, RefreshTokenDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetRefreshTokenQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public Task<RefreshTokenDto> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var refreshToken = _context
                .RefreshTokens
                .SingleOrDefault(t => t.Token == request.Token);

            return Task.FromResult(_mapper.Map<RefreshTokenDto>(refreshToken));
        }
    }
}