using Exceptions;
using Serilog.Core;
using static System.Net.Mime.MediaTypeNames;

namespace StockAppAssignment.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);

            }
            catch (Exception ex)
            {
                LogException(ex);
                //httpContext.Response.StatusCode = 500;
                //await httpContext.Response.WriteAsync("An unexpected fault happened. Try again later.");
                throw;
            }
        }
        private void LogException(Exception ex)
        {
            if(ex.InnerException.InnerException != null)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError("{ExceptionType}.{ExceptionMessage}",
                        ex.InnerException.GetType().ToString(),
                        ex.InnerException.Message);
                }
                else
                {
                    _logger.LogError("{ExceptionType}.{ExceptionMessage}",
                        ex.InnerException.InnerException.GetType().ToString(),
                        ex.InnerException.InnerException.Message);
                }
            }
            else
            {
                _logger.LogError("{ExceptionType}.{ExceptionMessage}",
                    ex.GetType().ToString(),
                    ex.Message);
            }
        }
    }
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
    
}
