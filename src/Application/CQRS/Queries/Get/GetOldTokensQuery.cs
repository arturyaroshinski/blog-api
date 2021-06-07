using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Yaroshinski.Blog.Application.Configuration;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Interfaces;

namespace Yaroshinski.Blog.Application.CQRS.Queries.Get
{
    public class GetOldTokensQuery : IRequest<List<RefreshTokenDto>>
    {
        public int AuthorId { get; set; }
    }

    public class GetOldTokensQueryHandler : IRequestHandler<GetOldTokensQuery, List<RefreshTokenDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _options;
        private readonly IAuthorizationService _authService;

        public GetOldTokensQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IOptions<AppSettings> options,
            IAuthorizationService authService)
        {
            _context = context;
            _mapper = mapper;
            _options = options.Value;
            _authService = authService;
        }
        
        public Task<List<RefreshTokenDto>> Handle(GetOldTokensQuery request, CancellationToken cancellationToken)
        {
            var oldTokens = _context.RefreshTokens
                .Where(t => t.AuthorId == request.AuthorId &&
                                !(t.Revoked == null && !(DateTime.UtcNow >= t.Expires)) &&
                                t.Created.AddDays(_options.RefreshTokenTtl) <= DateTime.UtcNow)
                .ToList();

            return Task.FromResult(_mapper.Map<List<RefreshTokenDto>>(oldTokens));
        }
    }
}