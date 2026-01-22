using Microsoft.EntityFrameworkCore;

namespace Lummo.Persistence.Extensions;

public static class DbContextExtensions
{
    public static void ApplyEntityConfigurations<TDataContext>(this ModelBuilder modelBuilder) where TDataContext : DbContext
    {
        var dbContextType = typeof(TDataContext);
        var entityConfigurationTypes = GetEntityConfigurations(dbContextType).ToList();

        entityConfigurationTypes.ForEach(type => modelBuilder.ApplyConfiguration((dynamic)Activator.CreateInstance(type)!));
    }

    public static IList<Type> GetEntityTypes(Type dbContextType)
    {
        var dbSetProperties = dbContextType
            .GetProperties()
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        return dbSetProperties
            .Select(p => p.PropertyType.GetGenericArguments()[0])
            .ToList();
    }

    public static IList<Type> GetEntityConfigurations(Type dbContextType)
    {
        var dbSetTypes = GetEntityTypes(dbContextType);

        var possibleEntityConfigurationTypes = dbSetTypes
            .Select(dbSetType => typeof(IEntityTypeConfiguration<>)
            .MakeGenericType(dbSetType))
            .ToList();

        var matchingConfigurationTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract &&
                            possibleEntityConfigurationTypes.Exists(configType => configType.IsAssignableFrom(type)))
            .ToList();

        return matchingConfigurationTypes;
    }
}
