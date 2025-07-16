namespace SmartAiChat.API.Middleware
{
    public class SecureHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecureHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");

            await _next(context);
        }
    }
}
