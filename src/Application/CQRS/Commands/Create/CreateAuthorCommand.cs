using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Application.Models;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.CQRS.Commands.Create
{
    public class CreateAuthorCommand : IRequest<Response<int>>
    {
        [Required] 
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EnumDataType(typeof(Role))]
        public string Role { get; set; }

        [Required]
        [EmailAddress] 
        public string Email { get; set; }

        [Required]
        [MinLength(6)] 
        public string Password { get; set; }

        [Required] 
        [Compare("Password")] 
        public string ConfirmPassword { get; set; }
    }

    public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, Response<int>>
    {
        private readonly IAuthorizationService _authService;
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateAuthorCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IAuthorizationService authService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<Response<int>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            
            if (_context.Authors.Any(a => a.Email == request.Email))
                throw new ExistingEmailException($"Email '{request.Email}' is already registered");

            var newAuthor = _mapper.Map<Author>(request);

            // TODO: delete, add data seed
            var isFirstAccount = !_context.Authors.Any();
            newAuthor.Role = isFirstAccount ? Role.Admin : Role.User;
            newAuthor.Created = DateTime.UtcNow;
            newAuthor.VerificationToken = _authService.RandomTokenString();
            newAuthor.PasswordHash = _authService.HashPassword(request.Password);

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