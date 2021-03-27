using Microsoft.AspNetCore.Mvc;
using Yaroshinski.Blog.Application.DTO;

namespace Yaroshinski.Blog.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected AuthorDto Author => (AuthorDto)HttpContext.Items["Author"];
    }
}