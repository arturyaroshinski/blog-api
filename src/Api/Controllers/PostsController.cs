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
using Yaroshinski.Blog.Application.Models;

namespace Yaroshinski.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<Response<List<PostDto>>> Get(GetPostsQuery command)
        {
            return await _mediator.Send(command);
        }

        [HttpPost]
        public async Task<Response<int>> Create(CreatePostCommand command)
        {
            return await _mediator.Send(command);
        }
        
        [HttpPost]
        public async Task<ActionResult> Update(int id, UpdatePostCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            
            await _mediator.Send(command);

            return NoContent();
        }
        
        [HttpPost]
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