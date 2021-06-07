using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using Yaroshinski.Blog.Api.Filters;
using Yaroshinski.Blog.Application.CQRS.Commands.Create;
using Yaroshinski.Blog.Application.CQRS.Commands.Delete;
using Yaroshinski.Blog.Application.CQRS.Commands.Update;
using Yaroshinski.Blog.Application.CQRS.Queries.Get;
using Yaroshinski.Blog.Domain.Entities;
using Yaroshinski.Blog.Application.Models;
using AutoMapper;

namespace Yaroshinski.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AuthorsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        // [HttpGet]
        // public async Task<ActionResult<List<AuthorDto>>> GetAll()
        // {
        //     var authors = _mediator.Send(new GetAuthorsQuery{});
        //     
        //     return Ok(response);
        // }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorResponse>> GetById(int id)
        {
            var authorDto = await _mediator.Send(new GetAuthorByIdQuery {Id = id});
            var authorResponse = _mapper.Map<AuthorResponse>(authorDto);

            return Ok(authorResponse);
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateAuthorCommand command)
        {
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(Create), response);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateAuthorCommand command)
        {
            // users can update their own author and admins can update any author
            if (id != Author.Id && Author.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            // users can delete their own author and admins can delete any author
            if (id != Author.Id && Author.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            await _mediator.Send(new DeleteAuthorCommand{Id = id});
            return NoContent();
        }
    }
}