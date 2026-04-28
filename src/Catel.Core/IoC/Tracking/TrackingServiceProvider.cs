namespace Catel.IoC;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

internal sealed class TrackingServiceProvider : IServiceProvider, IDisposable, IKeyedServiceProvider
{
    private readonly IServiceProvider _innerProvider;
    private readonly IKeyedServiceProvider? _innerKeyedProvider;
    private readonly ConcurrentDictionary<TrackedServiceKey, TrackedServiceMetadata> _resolvedServices = new();

    public TrackingServiceProvider(IServiceProvider innerProvider)
    {
        _innerProvider = innerProvider;
        _innerKeyedProvider = innerProvider as IKeyedServiceProvider;
    }

    public IReadOnlyDictionary<TrackedServiceKey, TrackedServiceMetadata> ResolvedServices => _resolvedServices;

    public object? GetService(Type serviceType)
    {
        var service = _innerProvider.GetService(serviceType);
        if (service is not null)
        {
            var key = new TrackedServiceKey
            {
                ServiceType = serviceType,
                ServiceKey = null
            };

            var metadata = new TrackedServiceMetadata
            {
                Key = key,
                ServiceInstance = service
            };

            _resolvedServices.TryAdd(key, metadata);
        }

        return service;
    }

    public object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        if (_innerKeyedProvider is null)
        {
            throw new InvalidOperationException("The inner service provider does not support keyed services.");
        }

        var service = _innerKeyedProvider.GetKeyedService(serviceType, serviceKey);
        if (service is not null)
        {
            var key = new TrackedServiceKey
            {
                ServiceType = serviceType,
                ServiceKey = serviceKey
            };

            var metadata = new TrackedServiceMetadata
            {
                Key = key,
                ServiceInstance = service
            };

            _resolvedServices.TryAdd(key, metadata);
        }

        return service;
    }

    public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
    {
        var keyedService = GetKeyedService(serviceType, serviceKey);
        if (keyedService is null)
        {
            throw new InvalidOperationException($"Required service '{serviceType.FullName}' with key '{serviceKey}' not registered");
        }

        return keyedService;
    }

    public void Dispose()
    {
        if (_innerProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
