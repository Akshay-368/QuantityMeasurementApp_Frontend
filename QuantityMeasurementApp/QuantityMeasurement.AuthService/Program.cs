using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurement.AuthService.Interfaces;
using QuantityMeasurement.AuthService.Services.Authentication;
using QuantityMeasurement.AuthService.Services.Security;
using QuantityMeasurement.AuthService.Persistence;

// Run this in powershell to register the user
// Invoke-RestMethod -Uri "http://localhost:5169/api/auth/register?username=test&password=123" -Method POST
// Run this for login the same user 
// Invoke-RestMethod -Uri "http://localhost:5169/api/auth/login?username=test&password=123" -Method POST



var builder = WebApplication.CreateBuilder(args);

// 🔹 Load env
DotNetEnv.Env.Load();

// 🔹 Add services FIRST
builder.Services.AddControllers();

// DI for AuthService and PasswordHasher
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// DB Context
builder.Services.AddDbContext<QuantityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IQuantityDbContext>(provider =>
    provider.GetRequiredService<QuantityDbContext>());

// JWT COnfiguration
var key = Environment.GetEnvironmentVariable("Jwt__Key") ?? throw new Exception ("JWT Key not found in environment variable"); // using thsi over builer.cofiguration because .env does not go into it automatically
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
                System.Text.Encoding.UTF8.GetBytes(key)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });




builder.Services.AddAuthorization();


// 🔹 Build AFTER everything is registered
var app = builder.Build(); // after this even di container is frozen

// 🔹 Middleware pipeline
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();