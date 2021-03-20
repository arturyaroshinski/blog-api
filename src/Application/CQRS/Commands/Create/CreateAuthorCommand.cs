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
    public class CreateAuthorCommand : IRequest<Response<int>>
    {
        public AuthorDto Model { get; set; }

    }

    public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, Response<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateAuthorCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response<int>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            var newAuthor = _mapper.Map<Author>(request.Model);
            try
            {
                await _context.Authors.AddAsync(newAuthor, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return Response.Fail(ex.Message, -1);
            }

            return Response.Ok("Author was successfully created", newAuthor.Id);
        }
    }
}
