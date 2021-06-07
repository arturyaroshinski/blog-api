using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yaroshinski.Blog.Api.Filters;
using Yaroshinski.Blog.Application.CQRS.Commands.Create;
using Yaroshinski.Blog.Application.CQRS.Commands.Delete;
using Yaroshinski.Blog.Application.CQRS.Commands.Update;
using Yaroshinski.Blog.Application.CQRS.Queries.Get;
using Yaroshinski.Blog.Application.DTO;

namespace Yaroshinski.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : BaseController
    {
        private readonly IMediator _mediator;

        public CommentsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<List<CommentDto>> Get(GetCommentsQuery command)
        {
            return await _mediator.Send(command);
        }
        
        [HttpGet("{id:int}")]
        public async Task<CommentDto> Get(int id)
        {
            return await _mediator.Send(new GetCommentByIdQuery{Id = id});
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<int>> Create(CreateCommentCommand command)
        {
            command.AuthorId = Author.Id;
            return await _mediator.Send(command);
        }
        
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult> Update(int id, UpdateCommentCommand command)
        {
            if (id != command.Id && id != Author.Id)
            {
                return BadRequest();
            }
            
            await _mediator.Send(command);

            return NoContent();
        }
        
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id, DeleteCommentCommand command)
        {
            if (id != command.Id && id != Author.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command);

            return NoContent();
        }
    }
}