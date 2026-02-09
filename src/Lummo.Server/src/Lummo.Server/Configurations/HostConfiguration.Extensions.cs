using FluentValidation;
using Lummo.Application.Common.EventBus.Brokers.Interfaces;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Application.Common.Notifications.Brokers.Interfaces;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Application.Common.Serializer;
using Lummo.Application.Common.Settings;
using Lummo.Application.Common.StorageFiles;
using Lummo.Application.Common.Verifications.Services.Interfaces;
using Lummo.Domain.Brokers;
using Lummo.Infrastructure.Common.Caching.Brokers;
using Lummo.Infrastructure.Common.EventBus.Brokers;
using Lummo.Infrastructure.Common.EventBus.Services;
using Lummo.Infrastructure.Common.Identity.Services;
using Lummo.Infrastructure.Common.Notifications.Brokers;
using Lummo.Infrastructure.Common.Notifications.Services;
using Lummo.Infrastructure.Common.RequestContexts;
using Lummo.Infrastructure.Common.Serializer;
using Lummo.Infrastructure.Common.Settings;
using Lummo.Infrastructure.Common.StorageFiles;
using Lummo.Infrastructure.Common.Verifications.Services;
using Lummo.Infrastructure.StorageFiles.Settings;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Interceptors;
using Lummo.Persistence.Repositories;
using Lummo.Persistence.Repositories.Interfaces;
using Lummo.Server.Data;
using Lummo.Server.Formatters;
using Lummo.Server.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

        builder.Services.AddSingleton<AccessTokenValidationMiddleware>();
        builder.Services.AddSingleton<GlobalExceptionHandlerMiddleware>();

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
        builder.Services.Configure<SmtpEmailSenderSettings>(
            builder.Configuration.GetSection(nameof(SmtpEmailSenderSettings)));

        builder.Services.Configure<TemplateRenderingSettings>(
            builder.Configuration.GetSection(nameof(TemplateRenderingSettings)));

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
            .AddScoped<IRoleRepository, RoleRepository>()
            .AddScoped<IUserRoleRepository, UserRoleRepository>()
            .AddScoped<IAccessTokenRepository, AccessTokenRepository>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        builder.Services
            .AddScoped<IUserService, UserService>()
            .AddScoped<IUserSettingsService, UserSettingsService>()
            .AddScoped<IRoleService, RoleService>()
            .AddScoped<IAccountService, AccountService>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IIdentitySecurityTokenGeneratorService, IdentitySecurityTokenGeneratorService>()
            .AddScoped<IIdentitySecurityTokenService, IdentitySecurityTokenService>()
            .AddScoped<IPasswordGeneratorService, PasswordGeneratorService>()
            .AddScoped<IPasswordHasherService, PasswordHasherService>()
            .AddScoped<IRoleProcessingService, RoleProcessingService>();

        return builder;
    }

    private static WebApplicationBuilder AddStorageFileInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<StorageFileSettings>(builder.Configuration.GetSection(nameof(StorageFileSettings)));

        builder.Services
            .AddScoped<IStorageFileRepository, StorageFileRepository>()
            .AddScoped<IStorageFileService, StorageFileService>();

        return builder;
    }

    private static WebApplicationBuilder AddVerificationInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<VerificationCodeSettings>(
            builder.Configuration.GetSection("VerificationSettings"));

        builder.Services.AddScoped<IUserInfoVerificationCodeRepository, UserInfoVerificationCodeRepository>();
        builder.Services.AddScoped<IUserInfoVerificationCodeService, UserInfoVerificationCodeService>();

        return builder;
    }

    private static WebApplicationBuilder AddRequestContextTools(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IRequestUserContextProvider, RequestUserContextProvider>();

        builder.Services.Configure<RequestUserContextSettings>(
            builder.Configuration.GetSection(nameof(RequestUserContextSettings)));

        return builder;
    }

    private static WebApplicationBuilder AddPersistence(this  WebApplicationBuilder builder)
    {
        builder.Services
            .AddScoped<UpdatePrimaryKeyInterceptor>()
            .AddScoped<UpdateAuditableInterceptor>()
            .AddScoped<UpdateSoftDeletionInterceptor>();

        builder.Services.AddDbContext<AppDbContext>((provider, options) =>
        {
            options
            .UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString"))
            .AddInterceptors(provider.GetRequiredService<UpdatePrimaryKeyInterceptor>(),
            provider.GetRequiredService<UpdateAuditableInterceptor>(),
            provider.GetRequiredService<UpdateSoftDeletionInterceptor>());
        });

        return builder;
    }

    private static WebApplicationBuilder AddExposers(this WebApplicationBuilder builder)
    {
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers(
            options =>
            {
                options.InputFormatters.Add(new TextInputFormatter());
            }
        );

        builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(nameof(ApiSettings)));

        return builder;
    }

    private static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options => options.AddPolicy("AllowSpecificOrigin",
            policy => policy
            .WithOrigins(builder.Configuration["ApiClientSettings:WebClientAddress"]!)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

        return builder;
    }

    private static WebApplicationBuilder AddDevTools(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(5188);
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return builder;
    }


    private static async ValueTask<WebApplication> MigrateDatabaseSchemasAsync(this WebApplication app)
    {
        var serviceScopeFactory = app.Services.GetRequiredKeyedService<IServiceScopeFactory>(null);

        await serviceScopeFactory.MigrateAsync<AppDbContext>();

        return app;
    }

    private static async ValueTask<WebApplication> SeedDataAsync(this WebApplication app)
    {
        var serviceScope = app.Services.CreateScope();
        await serviceScope.ServiceProvider.InitializeSeedAsync();

        return app;
    }
    private static WebApplication UseCors(this WebApplication app)
    {
        app.UseCors("AllowSpecificOrigin");

        return app;
    }

    private static WebApplication UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        return app;
    }

    private static WebApplication UseIdentityInfrastructure(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<AccessTokenValidationMiddleware>();

        return app;
    }

    private static WebApplication UseExposers(this WebApplication app)
    {
        app.MapControllers();

        return app;
    }

    private static WebApplication UseDevTools(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
