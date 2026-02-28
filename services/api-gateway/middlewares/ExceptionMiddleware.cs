using System.Net;
using api_gateway.Models;

namespace api_gateway.ExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsJsonAsync(
                    new ErrorResponse
                    {
                        Message = ex.Message,
                        StatusCode = HttpStatusCode.InternalServerError.ToString()
                    });
            }
        }


    }
}