using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Application.Models;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Queries.Get
{
    public class GetAuthorByIdQuery : IRequest<Response<AuthorDto>>
    {
        public int Id { get; set; }
    }

    public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, Response<AuthorDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAuthorByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public Task<Response<AuthorDto>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
        {
            var author = _context.Authors.Find(request.Id);

            if (author == null)
            {
                throw new NotFoundException(nameof(Author), request.Id);
            }

            var authorDto = _mapper.Map<AuthorDto>(author);

            return Task.FromResult(Response.Ok("Author was found successfully", authorDto));
        }
    }
}
