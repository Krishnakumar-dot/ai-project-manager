using System.Net;
using System.Text.Json;
using AiPmaPlatform.Application.Common.Models;
using FluentValidation;

namespace AiPmaPlatform.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                var message = string.Join(" ", ex.Errors.Select(e => e.ErrorMessage).Distinct());
                await WriteAsync(context, HttpStatusCode.BadRequest, message);
            }
            catch (Exception)
            {
                await WriteAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteAsync(HttpContext context, HttpStatusCode status, string message)
        {
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";
            var body = ApiResponse<object>.Fail(message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
        }
    }
}