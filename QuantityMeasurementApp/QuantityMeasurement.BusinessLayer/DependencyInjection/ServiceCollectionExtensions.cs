using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuantityMeasurement.BusinessLayer.Interfaces;
using QuantityMeasurement.BusinessLayer.Services;
using QuantityMeasurement.BusinessLayer.Services.Authentication;
using QuantityMeasurement.BusinessLayer.Services.Security;
using QuantityMeasurement.Infrastructure.Interfaces;
using QuantityMeasurement.Infrastructure.Persistence;
using QuantityMeasurement.Infrastructure.Repositories;

namespace QuantityMeasurement.BusinessLayer.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IQuantityService, QuantityService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IHistoryService, HistoryService>();

        services.AddScoped<IHistoryRepository, HistoryRepository>();
        services.AddScoped<IQuantityDbContext, QuantityDbContext>();

        services.AddMemoryCache();

        var redisConfig = configuration["Redis:Configuration"] ?? "localhost:6379";
        var instanceName = configuration["Redis:InstanceName"] ?? "QuantityMeasurementApp:";
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig;
            options.InstanceName = instanceName;
        });

        var connectionString = NormalizePostgresConnectionString(
            configuration.GetConnectionString("DefaultConnection")
        );

        services.AddDbContext<QuantityDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    private static string NormalizePostgresConnectionString(string? rawConnectionString)
    {
        if (string.IsNullOrWhiteSpace(rawConnectionString))
            throw new InvalidOperationException("DefaultConnection is not configured.");

        if (rawConnectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
            return rawConnectionString;

        if (Uri.TryCreate(rawConnectionString, UriKind.Absolute, out var uri)
            && (uri.Scheme.Equals("postgres", StringComparison.OrdinalIgnoreCase)
                || uri.Scheme.Equals("postgresql", StringComparison.OrdinalIgnoreCase)))
        {
            var database = uri.AbsolutePath.Trim('/');

            var username = string.Empty;
            var password = string.Empty;

            if (!string.IsNullOrWhiteSpace(uri.UserInfo))
            {
                var userParts = uri.UserInfo.Split(':', 2);
                username = Uri.UnescapeDataString(userParts[0]);
                if (userParts.Length > 1)
                    password = Uri.UnescapeDataString(userParts[1]);
            }

            var port = uri.IsDefaultPort ? 5432 : uri.Port;

            return $"Host={uri.Host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
        }

        return rawConnectionString;
    }
}
