using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Interfaces;

namespace Yaroshinski.Blog.Application.CQRS.Queries.Get
{
    public class GetCommentsQuery : IRequest<List<CommentDto>>
    {
        // TODO: add page size
        [Required]
        public int PostId { get; set; }
    }
    
    public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, List<CommentDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCommentsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<List<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var comments = await _context
                .Comments
                .Where(c => c.PostId == request.PostId)
                .ToListAsync(cancellationToken: cancellationToken);

            return _mapper.Map<List<CommentDto>>(comments);
        }
    }
}