using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Delete
{
    public class DeleteTokensCommand : IRequest
    {
        public List<RefreshTokenDto> Tokens { get; set; }
    }
    
    public class DeleteTokensCommandHandler : IRequestHandler<DeleteTokensCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DeleteTokensCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<Unit> Handle(DeleteTokensCommand request, CancellationToken cancellationToken)
        {
            var tokens = _mapper.Map<List<RefreshToken>>(request.Tokens);
            
            _context.RefreshTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}