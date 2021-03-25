using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Yaroshinski.Blog.Application.Exceptions;
using Yaroshinski.Blog.Domain.Exceptions;

namespace Yaroshinski.Blog.Api.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private bool _isServerError;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = error switch
                {
                    AppException _ => (int)HttpStatusCode.BadRequest,
                    NotFoundException _ => (int)HttpStatusCode.NotFound,
                    ExistingEmailException _ => (int)HttpStatusCode.BadRequest,
                    _ => ServerErrorCode()
                };

                var result = JsonSerializer.Serialize(new
                {
                    message = _isServerError ? "Server error" : error.Message
                });
                await response.WriteAsync(result);
            }
        }

        private int ServerErrorCode()
        {
            _isServerError = false;
            
            return (int) HttpStatusCode.InternalServerError;
        }
    }
}