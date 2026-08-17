namespace UserService.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyMiddleware(
        RequestDelegate next,
        IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ==================================================
        // ALLOW SWAGGER
        // ==================================================

        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }


        // ==================================================
        // ALLOW AUTH ENDPOINTS
        // ==================================================

        if (context.Request.Path.StartsWithSegments("/api/Auth"))
        {
            await _next(context);
            return;
        }


        // ==================================================
        // ALLOW USER REGISTRATION
        // ==================================================

        if (context.Request.Path.StartsWithSegments(
            "/api/Users/register"))
        {
            await _next(context);
            return;
        }


        // ==================================================
        // GET API KEY FROM REQUEST HEADER
        // ==================================================

        if (!context.Request.Headers.TryGetValue(
            "X-API-KEY",
            out var apiKey))
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(new
            {
                message = "API Key is missing."
            });

            return;
        }


        // ==================================================
        // GET VALID API KEY FROM APPSETTINGS
        // ==================================================

        var validApiKey =
            _configuration["ApiKey:Key"];


        // ==================================================
        // CHECK API KEY
        // ==================================================

        if (string.IsNullOrEmpty(validApiKey) ||
            apiKey != validApiKey)
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(new
            {
                message = "Invalid API Key."
            });

            return;
        }


        // ==================================================
        // CONTINUE REQUEST
        // ==================================================

        await _next(context);
    }
}