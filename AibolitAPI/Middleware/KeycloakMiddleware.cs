using AibolitAPI.Services;

namespace AibolitAPI.Middleware;

public class KeycloakMiddleware
{
    private readonly ILogger<KeycloakMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;

    public KeycloakMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory,
        ILogger<KeycloakMiddleware> logger)
    {
        _next = next;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authorizationHeader?.StartsWith("Bearer ") == true)
        {
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            using var scope = _scopeFactory.CreateScope();
            var keycloakService = scope.ServiceProvider.GetRequiredService<IKeycloakService>();
            await keycloakService.ProcessTokenAsync(token);
        }

        await _next(context);
    }
}