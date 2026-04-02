using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurement.API.Middleware; // For custom middleware
// [Convert]::ToBase64String((1..64 | ForEach-Object {Get-Random -Max 256})) - for key generation
// Also this package was added here : dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.8
// and this dotnet add package DotNetEnv
using QuantityMeasurement.BusinessLayer.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Net;
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var renderPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(renderPort))
{
    builder.WebHost.UseUrls($"http://*:{renderPort}");
}

// var key = builder.Configuration["Jwt:Key"] ; 
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

var configuredCorsOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")
    ?? builder.Configuration["Cors:AllowedOrigins"];

var allowedOrigins = (configuredCorsOrigins ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCors", policy =>
    {
        if (allowedOrigins.Length == 0)
        {
            policy.SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrWhiteSpace(origin)) return false;

                // Default local dev allow-list when no explicit CORS origins are configured.
                return origin.StartsWith("http://localhost", StringComparison.OrdinalIgnoreCase)
                       || origin.StartsWith("https://localhost", StringComparison.OrdinalIgnoreCase)
                       || origin.StartsWith("http://127.0.0.1", StringComparison.OrdinalIgnoreCase)
                       || origin.StartsWith("https://127.0.0.1", StringComparison.OrdinalIgnoreCase)
                       || origin.StartsWith("https://quantity-frontend.onrender.com", StringComparison.OrdinalIgnoreCase);
            });
        }
        else
        {
            policy.WithOrigins(allowedOrigins);
        }

        policy
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter( RateLimiterOptions =>
{
    RateLimiterOptions.RejectionStatusCode = 429;
    RateLimiterOptions.AddPolicy("fixedWindowLimiter" , HttpContext =>
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, partition =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(100),
                QueueLimit = 3,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });
}
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// register services
builder.Services.AddBusinessLayer(builder.Configuration);

var app = builder.Build();
// Once this line runs , the pipeline begins forming. Everything from up ahead  that use app.Use... , app.Map... , app.Run... ,becomes part of the request pipeline in ASP.NET Core which is running on kestrel.

// Swagger Middleware Componenet.

app.UseSwagger(); // To expose the OpenAPi json endpoint , something like this /swagger/v1/swagger.json
app.UseSwaggerUI(); // To expose the UI( basically serves the interactive swagger ui webpage in the browser when the endpoint is hit ( for current case which is the localhost port)) , something like this /swagger/index.html

app.UseGlobalException();

///<summary>
/// Creating a custom MiddleWare to logging the time between the request and response.
///</summary>
app.UseTimeLogging();


///<summary>
/// MiddleWare Components : Code that runs between the request and controller .
/// As in Asp.Netcore the middleware is added using app.Usesomthing() , app.MapSOmething() ,app.RunSomething(). And middleware is added to the app not the builder 
/// They form the HTTP request pipeline.
/// Now this one : app.UseHttpsRedirection();  is to redirect HTTP to HTTPS. ( like http://localhost:5228/ -> https://localhost:7051/swager , 
/// which is inside the launch-settings.json inside properties folder in the applcationUrl which is for the server from Kestrel 
/// to listen to which network ports  , so they only can contain protocol + host + port number and not the protocol + host + port + path )
///s</summary>
app.UseHttpsRedirection(); 

app.UseCors("AppCors");

app.UseRateLimiter();

app.UseAuthentication(); // This MUST come before authorization

///<summary>
/// This is the Middleware that handles the Authorization of the HTTP request.
/// And it comes in the second order in the pipeline after the HttpsRedirection middleware.
/// This checks if the user is allowed to access a resource to begin with. So basically it is utilised along with the JWT or controller attributes like [Authorize]
/// But currently this middleware is inactive but ready as there is no authentication yet.
///</summary>
app.UseAuthorization();

///<summary>
/// This is the Middleware that handles the routing of the HTTP request to controller actions .
/// And without it , every API endpoint would stop working.
/// For example if the request GET/api/history comes , then it maps it to the appropriate controler of the HistoryController class
///</summary>
app.MapControllers();

// And the flow of these middlewares is :
// app.UseHttpsRedirection(); -> app.UseAuthorization(); -> app.MapControllers(); and changing the order would break the authentication logic because the request would reach controllers before the authorization check even happens.

///<summary>
/// This is the Middleware that runs the application.
/// It means end the pipeline here . Do not call the next middleware. So no middleware after Run will ever execute.
/// And this starts the host that was created earlier by the WebApplication.CreateBuilder(args); as it internally starts the server from kestrel and begin listening on configured ports.
///</summary>
app.Run();
