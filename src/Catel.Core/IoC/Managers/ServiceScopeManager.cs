namespace Catel.IoC;

using Catel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

public class ServiceScopeManager : IServiceScopeManager
{
    private const string ScopeServiceCollectionKey = "ScopeServiceCollection";

    private readonly ILogger<ServiceScopeManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyList<IScopeServiceCollectionProvider> _serviceCollectionProviders;

    private readonly Dictionary<string, ServiceScopeState> _scopes = new();

    public ServiceScopeManager(ILogger<ServiceScopeManager> logger, IServiceProvider serviceProvider,
        IEnumerable<IScopeServiceCollectionProvider> serviceCollectionProviders)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _serviceCollectionProviders = serviceCollectionProviders.ToArray();
    }

    public ServiceScope? GetScope(string id)
    {
        if (!_scopes.TryGetValue(id, out var scopeState))
        {
            _logger.LogDebug("ScopeContext with ID {ScopeId} not found.", id);
            return null;
        }

        return scopeState.Scope;
    }

    public ServiceScope AddScope(ServiceScopeContext scopeContext)
    {
        if (_scopes.TryGetValue(scopeContext.Id, out var scopeState))
        {
            _logger.LogDebug("Returning existing scope");
            return scopeState.Scope;
        }

        _logger.LogDebug("Creating scope '{ScopeId}'", scopeContext.Id);

        var serviceCollection = new ServiceCollection();

        foreach (var serviceCollectionProvider in _serviceCollectionProviders)
        {
            serviceCollectionProvider.AddServices(serviceCollection, scopeContext.Id);
        }

        // Register a cloned service collection before adding parent registrations
        var explicitServices = new ServiceCollection();
        explicitServices.AddServiceCollectionRegistrations(serviceCollection);

        serviceCollection.AddKeyedSingleton<IServiceCollection>(ScopeServiceCollectionKey, explicitServices);

        if (!scopeContext.IsIsolated)
        {
            serviceCollection.AddParentServiceProviderRegistrations(_serviceProvider);
        }

        var scope = new ServiceScope
        {
            Id = scopeContext.Id,
            ServiceProvider = serviceCollection.BuildServiceProvider()
        };

        scopeState = new ServiceScopeState
        {
            Scope = scope,
            ScopeContext = scopeContext,
        };

        _scopes[scopeContext.Id] = scopeState;

        return scope;
    }

    public bool RemoveScope(string id)
    {
        if (!_scopes.TryGetValue(id, out var scopeState))
        {
            _logger.LogDebug("Cannot remove scope '{ScopeId}', scope does not exist", id);
            return false;
        }

        _logger.LogDebug("Removing scope '{ScopeId}'", id);

        // Only need to dispose manually if we don't dispose the service provider
        if (scopeState.ScopeContext.DisposeScopeServices &&
            !scopeState.ScopeContext.DisposeServiceProvider)
        {
            // Note: this might instantiate services that were not yet instantiated before

            var serviceProvider = scopeState.Scope.ServiceProvider;

            var serviceCollection = serviceProvider.GetServiceCollection(ScopeServiceCollectionKey);

            foreach (var serviceDescriptor in serviceCollection)
            {
                if (serviceDescriptor.ServiceType.IsGenericTypeDefinition)
                {
                    continue;
                }

                var service = serviceProvider.GetService(serviceDescriptor.ServiceType) as IDisposable;
                if (service is not null)
                {
                    service.Dispose();
                }
            }
        }

        if (scopeState.ScopeContext.DisposeServiceProvider)
        {
            _logger.LogDebug("Disposing service provider for scope '{ScopeId}'", id);

            var disposable = scopeState.Scope.ServiceProvider as IDisposable;
            if (disposable is not null)
            {
                disposable.Dispose();
            }
        }

        return true;
    }
}
