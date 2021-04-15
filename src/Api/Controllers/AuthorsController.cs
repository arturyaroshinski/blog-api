using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using Yaroshinski.Blog.Api.Filters;
using Yaroshinski.Blog.Application.CQRS.Commands.Create;
using Yaroshinski.Blog.Application.CQRS.Commands.Delete;
using Yaroshinski.Blog.Application.CQRS.Commands.Update;
using Yaroshinski.Blog.Application.CQRS.Queries.Get;
using Yaroshinski.Blog.Application.DTO;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : BaseController
    {
        private readonly IMediator _mediator;

        public AuthorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // [HttpGet]
        // public async Task<ActionResult<List<AuthorDto>>> GetAll()
        // {
        //     var authors = _mediator.Send(new GetAuthorsQuery{});
        //     
        //     return Ok(response);
        // }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorDto>> GetById(int id)
        {
            var author = await _mediator.Send(new GetAuthorByIdQuery {Id = id});
            return Ok(author);
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