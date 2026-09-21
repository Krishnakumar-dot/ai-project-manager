using System.Net;
using System.Text.Json;
using AiPmaPlatform.Application.Common.Models;

namespace AiPmaPlatform.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var errorResponse = ApiResponse<object>.Fail("An unexpected error occurred.");
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
        }
    }
}