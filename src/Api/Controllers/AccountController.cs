using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Yaroshinski.Blog.Api.Models;
using Yaroshinski.Blog.Api.Filters;
using Yaroshinski.Blog.Api.Services;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly IMapper _mapper;

        public AuthorsController(
            IAuthorService authorService,
            IMapper mapper)
        {
            _authorService = authorService;
            _mapper = mapper;
        }

        [HttpPost("authenticate")]
        public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var response = _authorService.Authenticate(model, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public ActionResult<AuthenticateResponse> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _authorService.RefreshToken(refreshToken, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public IActionResult RevokeToken(RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            // users can revoke their own tokens and admins can revoke any tokens
            if (// TODO: extract !Author.OwnsToken(token) &&
                Author.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            _authorService.RevokeToken(token, IpAddress());
            return Ok(new { message = "Token revoked" });
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest model)
        {
            _authorService.Register(model, Request.Headers["origin"]);
            return Ok(new { message = "Registration successful, please check your email for verification instructions" });
        }

        [HttpPost("verify-email")]
        public IActionResult VerifyEmail(VerifyEmailRequest model)
        {
            _authorService.VerifyEmail(model.Token);
            return Ok(new { message = "Verification successful, you can now login" });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordRequest model)
        {
            _authorService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [HttpPost("validate-reset-token")]
        public IActionResult ValidateResetToken(ValidateResetTokenRequest model)
        {
            _authorService.ValidateResetToken(model);
            return Ok(new { message = "Token is valid" });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequest model)
        {
            _authorService.ResetPassword(model);
            return Ok(new { message = "Password reset successful, you can now login" });
        }

        [Authorize(Role.Admin)]
        [HttpGet]
        public ActionResult<List<AuthorResponse>> GetAll()
        {
            var authors = _authorService.GetAll();
            var response = _mapper.Map<List<AuthorResponse>>(authors);
            
            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public ActionResult<AuthorResponse> GetById(int id)
        {
            // users can get their own author and admins can get any author
            if (id != Author.Id && Author.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            var author = _authorService.GetById(id);
            return Ok(author);
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        public ActionResult<AuthorResponse> Create(CreateAuthorRequest model)
        {
            var author = _authorService.Create(model);
            return Ok(author);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public ActionResult<AuthorResponse> Update(int id, UpdateAuthorRequest model)
        {
            // users can update their own author and admins can update any author
            if (id != Author.Id && Author.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            // only admins can update role
            if (Author.Role != Role.Admin)
                model.Role = null;

            var author = _authorService.Update(id, model);
            return Ok(author);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            // users can delete their own author and admins can delete any author
            if (id != Author.Id && Author.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            _authorService.Delete(id);
            return Ok(new { message = "Author deleted successfully" });
        }

        // helper methods

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private Author Author => (Author)HttpContext.Items["Author"];
        
        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}