namespace Catel
{
    using System;
    using Catel.IoC;
    using Catel.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    public static class IServiceProviderExtensions
    {
        public static bool IsRegistered<TService>(this IServiceProvider serviceProvider)
        {
            return IsRegistered(serviceProvider, typeof(TService));
        }

        public static bool IsRegistered(this IServiceProvider serviceProvider, Type serviceType)
        {
            var serviceChecker = serviceProvider.GetRequiredService<IServiceProviderIsService>();

            return serviceChecker.IsService(serviceType);
        }

        public static void CreateTypesThatMustBeConstructedAtStartup(this IServiceProvider serviceProvider)
        {
            var serviceCollection = serviceProvider.GetKeyedService<IServiceCollection>("ConstructAtStartup");
            if (serviceCollection is not null)
            {
                foreach (var service in serviceCollection)
                {
                    // Only singletons make sense
                    if (service.Lifetime != ServiceLifetime.Singleton)
                    {
                        continue;
                    }

                    if (service.ImplementationType?.ImplementsInterfaceEx<IConstructAtStartup>() ?? false)
                    {
                        var key = service.ServiceKey;
                        if (key is null)
                        {
                            _ = serviceProvider.GetRequiredService(service.ServiceType);
                        }
                        else
                        {
                            _ = serviceProvider.GetRequiredKeyedService(service.ServiceType, key);
                        }
                    }
                }
            }
        }
    }
}
