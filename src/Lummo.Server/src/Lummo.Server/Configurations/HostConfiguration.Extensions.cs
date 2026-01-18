using Lummo.Application.Common.Serializer;
using Lummo.Application.Common.Services.Interfaces;
using Lummo.Infrastructure.Common.Caching.Brokers;
using Lummo.Infrastructure.Common.Serializer;
using Lummo.Infrastructure.Services;
using Lummo.Infrastructure.Settings;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.DataContexts;
using Lummo.Server.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;
using System.Text;
using Lummo.Application.Common.EventBus.Brokers.Interfaces;
using Lummo.Infrastructure.Common.EventBus.Brokers;
using Lummo.Infrastructure.Common.EventBus.Services;
using FluentValidation;
using System.Runtime.CompilerServices;
using Lummo.Persistence.Repositories.Interfaces;
using Lummo.Persistence.Repositories;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Infrastructure.Notifications.Services;
using Lummo.Application.Common.Notifications.Brokers.Interfaces;
using Lummo.Infrastructure.Notifications.Brokers;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Infrastructure.Common.Identity.Services;

namespace Lummo.Server.Configurations;

public static partial class HostConfiguration
{
    private static readonly ICollection<Assembly> Assemblies;

    static HostConfiguration()
    {
        Assemblies = typeof(HostConfiguration).Assembly.GetReferencedAssemblies()
            .Select(Assembly.Load).ToList();
        Assemblies.Add(typeof(HostConfiguration).Assembly);
    }

    public static WebApplicationBuilder AddCustomLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        var logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration).CreateLogger();

        builder.Host.UseSerilog(logger);

        return builder;
    }

    private static WebApplicationBuilder AddMediator(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(Assemblies.ToArray()); });
        return builder;
    }

    private static WebApplicationBuilder AddMapping(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(Assemblies);
        return builder;
    }

    private static WebApplicationBuilder AddCaching(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection(nameof(CacheSettings)));
        builder.Services.AddStackExchangeRedisCache(
            options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
                options.InstanceName = "Lummo.CacheMemory";
            });

        builder.Services.AddSingleton<ICacheBroker, RedisDistributedCacheBroker>();

        var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings is not configured.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
            options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtSettings.ValidateIssuer,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    ValidAudience = jwtSettings.ValidAudience,
                    ValidateAudience = jwtSettings.ValidateAudience,
                    ValidateLifetime = jwtSettings.ValidateLifetime,
                    ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secretkey))
                };
            });

        builder.Services.AddSingleton<AccessTokenValidationMiddleware>();

        return builder;
    }

    private static WebApplicationBuilder AddEventBus(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqConnectionSettings>(builder.Configuration.GetSection(
            nameof(RabbitMqConnectionSettings)));

        builder.Services.AddSingleton<IRabbitMqConnectionProvider, RabbitMqConnectionProvider>()
            .AddSingleton<IEventBusBroker, RabbitMqEventBusBroker>();

        builder.Services.AddHostedService<EventBusBackgroundService>();

        return builder;
    }

    private static WebApplicationBuilder AddValidators(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblies(Assemblies);
        return builder;
    }

    private static WebApplicationBuilder AddMappers(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(Assemblies);
        return builder;
    }

    private static WebApplicationBuilder AddSerializers(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IJsonSerializationSettingsProvider, JsonSerializationSettingsProvider>();

        return builder;
    }

    private static WebApplicationBuilder AddNotificationInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddScoped<IEmailTemplateRepository, EmailTemplateRepository>()
            .AddScoped<IEmailHistoryRepository, EmailHistoryRepository>();

        builder.Services
            .AddScoped<IEmailTemplateService, EmailTemplateService>()
            .AddScoped<IEmailRenderingService, EmailRenderingService>()
            .AddScoped<IEmailHistoryService, EmailHistoryService>();

        builder.Services
            .AddScoped<IEmailSenderBroker, SmtpEmailSenderBroker>();

        builder.Services
            .AddScoped<IEmailSenderService, EmailSenderService>();

        return builder;
    }

    private static WebApplicationBuilder AddIdentityInfrastucture(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
        builder.Services.Configure<PasswordValidationSettings>(builder.Configuration.GetSection(nameof(PasswordValidationSettings)));
        builder.Services.Configure<ValidationSettings>(builder.Configuration.GetSection(nameof(ValidationSettings)));

        builder.Services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IUserSettingsRepository, UserSettingsRepository>()
            .AddScoped<IAccessTokenRepository, AccessTokenRepository>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        builder.Services
            .AddScoped<IUserService, UserService>()
            .AddScoped<IUserSettingsService, UserSettingsService>()
            .AddScoped<IIdentitySecurityTokenService, IdentitySecurityTokenService>()
            .AddScoped<IPasswordGeneratorService, PasswordGeneratorService>()
            .AddScoped<IPasswordHasherService, PasswordHasherService>();

        return builder;
    }

    //private static WebApplicationBuilder AddStorageFileInfrastructure(this WebApplicationBuilder builder)
    //{
    //    builder.Services.Configure<StorageFileSettings>
    //}
}
