namespace Lummo.Server.Configurations;

public static partial class HostConfiguration
{
    public static ValueTask<WebApplicationBuilder> ConfigureAsync(this WebApplicationBuilder builder)
    {
        builder
            .AddCustomLogging()
            .AddMediator()
            .AddCaching()
            .AddEventBus()
            .AddValidators()
            .AddMappers()
            .AddSerializers()
            .AddPersistence()
            .AddDevTools()
            .AddStorageFileInfrastructure()
            .AddIdentityInfrastucture()
            .AddRequestContextTools()
            .AddVerificationInfrastructure()
            .AddNotificationInfrastructure()
            .AddCors()
            .AddExposers();

        return new(builder);
    }

    public static async ValueTask<WebApplication> ConfigureAsync(this WebApplication app)
    {
        await app.MigrateDatabaseSchemasAsync();
        await app.SeedDataAsync();
        app
            .UseGlobalExceptionHandler()
            .UseDevTools()
            .UseCors()
            .UseIdentityInfrastructure()
            .UseExposers()
            .UseStaticFiles();

        return app;
    }
}
