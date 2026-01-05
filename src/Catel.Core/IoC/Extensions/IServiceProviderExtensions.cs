namespace Catel
{
    using System;
    using System.Collections.Generic;
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
            var serviceCollection = serviceProvider.GetServiceCollection();

            foreach (var service in serviceCollection)
            {
                // Only singletons make sense
                if (service.Lifetime != ServiceLifetime.Singleton)
                {
                    continue;
                }

                if (service.ImplementationType?.ImplementsInterfaceEx<IConstructAtStartup>() ?? false)
                {
                    IConstructAtStartup? instance = null;

                    var key = service.ServiceKey;
                    if (key is null)
                    {
                        instance = serviceProvider.GetRequiredService(service.ServiceType) as IConstructAtStartup;
                    }
                    else
                    {
                        instance = serviceProvider.GetRequiredKeyedService(service.ServiceType, key) as IConstructAtStartup;
                    }

                    if (instance is IInitializeAtStartup initializeAtStartup)
                    {
                        initializeAtStartup.Initialize();
                    }
                }
            }
        }

        public static IReadOnlyList<ServiceDescriptor> GetServiceDescriptors<T>(this IServiceProvider serviceProvider)
        {
            return GetServiceDescriptors(serviceProvider, typeof(T));
        }

        public static IReadOnlyList<ServiceDescriptor> GetServiceDescriptors(this IServiceProvider serviceProvider, Type type)
        {
            var serviceCollection = serviceProvider.GetServiceCollection();

            var serviceDescriptors = new List<ServiceDescriptor>();

            foreach (var service in serviceCollection)
            {
                if (service.ServiceType != type)
                {
                    continue;
                }

                serviceDescriptors.Add(service);
            }

            return serviceDescriptors;
        }

        public static IServiceCollection GetServiceCollection(this IServiceProvider serviceProvider)
        {
            var serviceCollection = serviceProvider.GetKeyedService<IServiceCollection>("CatelServiceCollection");
            if (serviceCollection is not null)
            {
                return serviceCollection;
            }

            return new ServiceCollection();
        }
    }
}
