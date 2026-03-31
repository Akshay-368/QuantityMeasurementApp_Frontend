using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurement.QuantityService.Persistence;
using QuantityMeasurement.QuantityService.Repositories;
using QuantityMeasurement.QuantityService.Interfaces;
using QuantityMeasurement.QuantityService.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Load env
DotNetEnv.Env.Load();

// 🔹 Add Controllers
builder.Services.AddControllers();

// 🔹 DB Context (OWN DB)
builder.Services.AddDbContext<QuantityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

// 🔹 DI (your services)
builder.Services.AddScoped<IQuantityService, QuantityService>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();

// 🔹 JWT VALIDATION ONLY (no issuing here)
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

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// 🔹 Build
var app = builder.Build();

// 🔹 Middleware
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();