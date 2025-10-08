
using System.Text.Json;

namespace RestaurantsApi.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception and send it to the database
                _logger.LogError(ex, "An unhandled exception occurred.");

                // Return a generic error response
                context.Response.StatusCode = 500; // Internal Server Error
                context.Response.ContentType = "application/json";
                var response = new
                {
                    message = "An unexpected error occurred. Please try again later.",
                    error = ex.Message,
                    traceId = context.TraceIdentifier
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }

    }
}