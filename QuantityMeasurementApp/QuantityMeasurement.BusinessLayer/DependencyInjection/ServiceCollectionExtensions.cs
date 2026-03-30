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

        services.AddDbContext<QuantityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}
