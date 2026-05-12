namespace Catel;

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Registers a service as keyed singleton *and* as singleton, allowing the keyed singleton to be resolved
    /// by enumeration as well.
    /// </summary>
    /// <remarks>
    /// This method / idea comes from https://stackoverflow.com/questions/78220267/get-all-implementations-of-a-given-type-from-iserviceprovider-regardless-of-if-t
    /// <para />
    /// Use with care.
    /// </remarks>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="serviceCollection"></param>
    /// <param name="key"></param>
    public static void AddKeyedAndDefaultSingleton<TService, TImplementation>(this IServiceCollection serviceCollection, string key)
        where TService : class
        where TImplementation : class, TService
    {
        serviceCollection.AddKeyedSingleton<TService, TImplementation>(key);
        serviceCollection.AddSingleton(sp => sp.GetRequiredKeyedService<TService>(key));
    }

    public static void AddParentServiceProviderRegistrations(this IServiceCollection serviceCollection, IServiceProvider serviceProvider)
    {
        var serviceProviderServiceCollection = serviceProvider.GetServiceCollection();

        foreach (var serviceDescriptor in serviceProviderServiceCollection)
        {
            // Note: open generics are not supported, the validation of the service collection will fail (not sure why),
            // but we need to skip in that case
            var serviceType = serviceDescriptor.ServiceType;
            if (serviceType.IsGenericTypeDefinition)
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
        foreach (var serviceDescriptor in otherServiceCollection)
        {
            serviceCollection.Add(serviceDescriptor);
        }
    }

    public static bool IsRegistered<TService>(this IServiceCollection serviceCollection)
    {
        return serviceCollection.IsRegistered(typeof(TService));
    }

    public static bool IsRegistered(this IServiceCollection serviceCollection, Type serviceType)
    {
        return serviceCollection.Any(x => x.ServiceType == serviceType);
    }
}
