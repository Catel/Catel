namespace Catel
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Runtime.Serialization.Json;

    /// <summary>
    /// Core module which allows the registration of default services in the service locator.
    /// </summary>
    public static class SerializationJsonModule
    {
        public static IServiceCollection AddCatelSerializationJsonServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddCatelCoreServices();

            serviceCollection.TryAddSingleton<IJsonSerializer, JsonSerializer>();

            return serviceCollection;
        }
    }
}
