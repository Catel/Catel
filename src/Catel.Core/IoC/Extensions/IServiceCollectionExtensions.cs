namespace Catel
{
    using System;
    using System.Linq;
    using Catel.Collections;
    using Catel.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    public static class IServiceCollectionExtensions
    {
        public static void AddServiceProviderRegistrations(this IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            var serviceProviderServiceCollection = serviceProvider.GetServiceCollection();

            foreach (var serviceDescriptor in serviceProviderServiceCollection)
            {
                // Note: open generics are not supported, the validation of the service collection will fail (not sure why),
                // but we need to skip in that case
                var serviceType = serviceDescriptor.ServiceType;
                if (serviceType.IsGenericTypeDefinitionEx())
                {
                    // Register as-is
                    serviceCollection.Add(serviceDescriptor);
                    continue;
                }

                var serviceKey = serviceDescriptor.ServiceKey;
                if (serviceKey is not null)
                {
                    switch (serviceDescriptor.Lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            serviceCollection.AddKeyedSingleton(serviceDescriptor.ServiceType, serviceKey, (x, y) => serviceProvider.GetRequiredKeyedService(serviceDescriptor.ServiceType, y));
                            break;

                        case ServiceLifetime.Transient:
                            serviceCollection.AddKeyedTransient(serviceDescriptor.ServiceType, serviceKey, (x, y) => serviceProvider.GetRequiredKeyedService(serviceDescriptor.ServiceType, y));
                            break;

                        case ServiceLifetime.Scoped:
                            serviceCollection.AddKeyedScoped(serviceDescriptor.ServiceType, serviceKey, (x, y) => serviceProvider.GetRequiredKeyedService(serviceDescriptor.ServiceType, y));
                            break;
                    }
                }
                else 
                {
                    switch (serviceDescriptor.Lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            serviceCollection.AddSingleton(serviceDescriptor.ServiceType, x => serviceProvider.GetRequiredService(serviceDescriptor.ServiceType));
                            break;

                        case ServiceLifetime.Transient:
                            serviceCollection.AddTransient(serviceDescriptor.ServiceType, x => serviceProvider.GetRequiredService(serviceDescriptor.ServiceType));
                            break;

                        case ServiceLifetime.Scoped:
                            serviceCollection.AddScoped(serviceDescriptor.ServiceType, x => serviceProvider.GetRequiredService(serviceDescriptor.ServiceType));
                            break;
                    }
                }
            }
        }
        public static void AddServiceCollectionRegistrations(this IServiceCollection serviceCollection, IServiceCollection otherServiceCollection)
        {
            otherServiceCollection.ForEach(x => serviceCollection.Add(x));
        }

        public static bool IsRegistered<TService>(this IServiceCollection serviceCollection)
        {
            return serviceCollection.Any(x => x.ServiceType == typeof(TService));
        }
    }
}
