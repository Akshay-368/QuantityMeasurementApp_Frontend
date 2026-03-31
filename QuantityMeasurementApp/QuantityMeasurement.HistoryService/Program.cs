using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurement.HistoryService.Interfaces;
using QuantityMeasurement.HistoryService.Persistence;
using QuantityMeasurement.HistoryService.Repositories;
using QuantityMeasurement.HistoryService.Services;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

builder.Services.AddControllers();

builder.Services.AddDbContext<QuantityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var redisConfig = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
var instanceName = builder.Configuration["Redis:InstanceName"] ?? "QuantityMeasurementApp:";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConfig;
    options.InstanceName = instanceName;
});

builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<IHistoryService, HistoryService>();

var key = Environment.GetEnvironmentVariable("Jwt__Key")
          ?? throw new Exception("JWT Key not found");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("Jwt__Issuer"),
            ValidAudience = Environment.GetEnvironmentVariable("Jwt__Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = 429;
    rateLimiterOptions.AddPolicy("fixedWindowLimiter", httpContext =>
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(100),
                QueueLimit = 3,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
