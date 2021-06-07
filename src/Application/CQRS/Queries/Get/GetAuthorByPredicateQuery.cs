using System;
using System.Linq;
using System.Linq.Expressions;
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
    public class GetAuthorByPredicateQuery : IRequest<AuthorDto>
    {
        public Expression<Func<Author, bool>> Predicate { get; set; }
    }

    public class GetAuthorByPredicateQueryHandler : IRequestHandler<GetAuthorByPredicateQuery, AuthorDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAuthorByPredicateQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public Task<AuthorDto> Handle(GetAuthorByPredicateQuery request, CancellationToken cancellationToken)
        {
            var author = _context.Authors.FirstOrDefault(request.Predicate);

            if (author == null) throw new NotFoundException(nameof(Author), request.Predicate);

            return Task.FromResult(_mapper.Map<AuthorDto>(author));
        }
    }
}