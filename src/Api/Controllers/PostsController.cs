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
    public class PostsController : BaseController
    {
        private readonly IMediator _mediator;

        public PostsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<List<PostDto>> Get(GetPostsQuery command)
        {
            return await _mediator.Send(command);
        }
        
        [HttpGet("{id:int}")]
        public async Task<PostDto> Get(int id)
        {
            return await _mediator.Send(new GetPostByIdQuery{Id = id});
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<int>> Create(CreatePostCommand command)
        {
            command.AuthorId = Author.Id;
            return await _mediator.Send(command);
        }
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Update(int id, UpdatePostCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            
            await _mediator.Send(command);

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id, DeletePostCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command);

            return NoContent();
        }
    }
}