using System.Net;
using System.Text.Json;
using FixNear.Application.Exceptions;

namespace FixNear.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                int statusCode;
                object response;
                bool expected;

                switch (ex)
                {
                    case NotFoundException:
                        statusCode = (int)HttpStatusCode.NotFound;
                        response = new { message = ex.Message };
                        expected = true;
                        break;

                    case BadRequestException:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        response = new { message = ex.Message };
                        expected = true;
                        break;

                    case ConflictException:
                        statusCode = (int)HttpStatusCode.Conflict;
                        response = new { message = ex.Message };
                        expected = true;
                        break;

                    case UnauthorizedException:
                        statusCode = (int)HttpStatusCode.Unauthorized;
                        response = new { message = ex.Message };
                        expected = true;
                        break;

                    default:
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        response = new { message = "Something went wrong" };
                        expected = false;
                        break;
                }

                if (expected)
                    _logger.LogInformation("{Exception}: {Message}", ex.GetType().Name, ex.Message);
                else
                    _logger.LogError(ex, ex.Message);

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
