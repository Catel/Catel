namespace Catel
{
    using IoC;
    using Runtime.Serialization.Json;

    /// <summary>
    /// Core module which allows the registration of default services in the service locator.
    /// </summary>
    public class SerializationJsonModule : IServiceLocatorInitializer
    {
        /// <summary>
        /// Initializes the specified service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public void Initialize(IServiceLocator serviceLocator)
        {
            ArgumentNullException.ThrowIfNull(serviceLocator);

            serviceLocator.RegisterType<IJsonSerializer, JsonSerializer>();
        }
    }
}
