
namespace Yuggenda.Api.Middleware;

public class RequestIdMiddleware
{
    private const string HeaderName = "X-Request-Id";
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestIdMiddleware> _logger;

    public RequestIdMiddleware(
        RequestDelegate next,
        ILogger<RequestIdMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
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

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestId"] = requestId
        }))
        {
            await _next(context);
        }

        context.Response.Headers[HeaderName] = requestId;

        await _next(context);
    }
}
