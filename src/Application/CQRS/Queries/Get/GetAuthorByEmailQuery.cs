using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Queries.Get
{
    public class GetAuthorByEmailQuery : IRequest<AuthorDto>
    {
        public string Email { get; set; }
    }

    public class GetAuthorByEmailQueryHandler : IRequestHandler<GetAuthorByEmailQuery, AuthorDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAuthorByEmailQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public async Task<AuthorDto> Handle(GetAuthorByEmailQuery request, CancellationToken cancellationToken)
        {
            var author = await _context.Authors.FindAsync(request.Email);

            if (author == null)
            {
                throw new NotFoundException(nameof(Author), request.Email);
            }

            return _mapper.Map<AuthorDto>(author);
        }
    }
}