using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MediatR;
using Yaroshinski.Blog.Api.Filters;
using Yaroshinski.Blog.Application.CQRS.Commands.Create;
using Yaroshinski.Blog.Application.CQRS.Commands.Update;
using Yaroshinski.Blog.Application.CQRS.Queries.Get;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Application.Models;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizationService;

        public AccountController(
            IMediator mediator,
            IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _authorizationService = authorizationService;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateCommand command)
        {
            var response = await _mediator.Send(command);
            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthenticateResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { message = "Token is required" });

            var response = await _mediator.Send(new RefreshTokenCommand
            {
                Token = refreshToken,
                IpAddress = IpAddress()
            });

            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(RevokeTokenCommand command)
        {
            // accept token from request body or cookie
            var token = command.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            // users can revoke their own tokens and admins can revoke any tokens
            if (await _authorizationService.OwnsToken(token) && Author.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            command.Token = token;
            command.IpAddress = IpAddress();
            await _mediator.Send(command);

            return Ok(new { message = "Token revoked" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateAuthorCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "Registration successful, please check your email for verification instructions" });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "Verification successful, you can now login" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken(string token)
        {
            await _mediator.Send(new ValidateResetTokenQuery { Token = token });
            return Ok(new { message = "Token is valid" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];

            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
