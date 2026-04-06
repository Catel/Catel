namespace Catel.IoC;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

internal sealed class TrackingServiceProvider : IServiceProvider, IDisposable, IKeyedServiceProvider
{
    private readonly IServiceProvider _innerProvider;
    private readonly IKeyedServiceProvider? _innerKeyedProvider;
    private readonly ConcurrentDictionary<Type, object> _resolvedServices = new();

    public TrackingServiceProvider(IServiceProvider innerProvider)
    {
        _innerProvider = innerProvider;
        _innerKeyedProvider = innerProvider as IKeyedServiceProvider;
    }

    public IReadOnlyDictionary<Type, object> ResolvedServices => _resolvedServices;

    public object? GetService(Type serviceType)
    {
        var service = _innerProvider.GetService(serviceType);
        if (service is not null)
        {
            _resolvedServices.TryAdd(serviceType, service);
        }

        return service;
    }

    public object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        if (_innerKeyedProvider is null)
        {
            throw new InvalidOperationException("The inner service provider does not support keyed services.");
        }

        return _innerKeyedProvider.GetKeyedService(serviceType, serviceKey);
    }

    public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
    {
        if (_innerKeyedProvider is null)
        {
            throw new InvalidOperationException("The inner service provider does not support keyed services.");
        }

        return _innerKeyedProvider.GetRequiredKeyedService(serviceType, serviceKey);
    }

    public void Dispose()
    {
        if (_innerProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
