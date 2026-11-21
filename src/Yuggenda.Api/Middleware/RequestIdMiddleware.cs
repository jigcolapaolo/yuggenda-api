
namespace Yuggenda.Api.Middleware;

public class RequestIdMiddleware
{
    private const string HeaderName = "X-Request-Id";
    private readonly RequestDelegate _next;

    public RequestIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(requestId)
            || requestId.Length > 64
            || !requestId.All(c =>
                char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.'))
        {
            requestId = Guid.NewGuid().ToString("N");
        }

        context.TraceIdentifier = requestId;

        context.Response.Headers[HeaderName] = requestId;

        await _next(context);
    }
}
