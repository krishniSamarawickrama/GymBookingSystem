using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// JWT CONFIGURATION
// ======================================================

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience is missing.");


// ======================================================
// JWT AUTHENTICATION
// ======================================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });


// ======================================================
// AUTHORIZATION
// ======================================================

builder.Services.AddAuthorization();


// ======================================================
// CORS
// ======================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// ======================================================
// RATE LIMITING
// ======================================================

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("GatewayPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey:
                httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown",

            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,

                Window = TimeSpan.FromMinutes(1),

                QueueLimit = 0,

                AutoReplenishment = true
            }));
});


// ======================================================
// YARP REVERSE PROXY
// ======================================================

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy")
    );


// ======================================================
// BUILD APPLICATION
// ======================================================

var app = builder.Build();


// ======================================================
// CORS
// ======================================================

app.UseCors("ClientPolicy");


// ======================================================
// RATE LIMITING
// ======================================================

app.UseRateLimiter();


// ======================================================
// JWT AUTHENTICATION
// ======================================================

app.UseAuthentication();


// ======================================================
// AUTHORIZATION
// ======================================================

app.UseAuthorization();


// ======================================================
// GATEWAY HEALTH CHECK
// ======================================================

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        service = "GymBookingSystem API Gateway",
        status = "Running",
        message = "API Gateway is working successfully."
    });
});


// ======================================================
// REVERSE PROXY
// ======================================================

app.MapReverseProxy()
    .RequireRateLimiting("GatewayPolicy");


// ======================================================
// START APPLICATION
// ======================================================

app.Run();