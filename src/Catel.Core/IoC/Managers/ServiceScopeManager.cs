namespace Catel.IoC;

using Catel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

#if NET9_0_OR_GREATER
using System.Threading;
#endif

public class ServiceScopeManager : IServiceScopeManager
{
    private const string ScopeServiceCollectionKey = "ScopeServiceCollection";

    private readonly ILogger<ServiceScopeManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyList<IScopeServiceCollectionProvider> _serviceCollectionProviders;

#if NET9_0_OR_GREATER
    private readonly Lock _scopesLock = new();
#else
    private readonly object _scopesLock = new();
#endif

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
        lock (_scopesLock)
        {
            if (!_scopes.TryGetValue(id, out var scopeState))
            {
                _logger.LogDebug("ScopeContext with ID {ScopeId} not found.", id);
                return null;
            }

            return scopeState.Scope;
        }
    }

    public ServiceScope AddScope(ServiceScopeContext scopeContext)
    {
        lock (_scopesLock)
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
                ServiceProvider = new TrackingServiceProvider(serviceCollection.BuildServiceProvider())
            };

            scopeState = new ServiceScopeState
            {
                Scope = scope,
                ScopeContext = scopeContext,
            };

            _scopes[scopeContext.Id] = scopeState;

            return scope;
        }
    }

    public bool RemoveScope(string id)
    {
        ServiceScopeState scopeState;

        lock (_scopesLock)
        {
            if (!_scopes.Remove(id, out scopeState!))
            {
                _logger.LogDebug("Cannot remove scope '{ScopeId}', scope does not exist", id);
                return false;
            }
        }

        _logger.LogDebug("Removing scope '{ScopeId}'", id);

        // Only need to dispose manually if we don't dispose the service provider
        if (scopeState.ScopeContext.DisposeScopeServices &&
            !scopeState.ScopeContext.DisposeServiceProvider)
        {
            var serviceProvider = scopeState.Scope.ServiceProvider;

            if (serviceProvider is TrackingServiceProvider trackingProvider)
            {
                var serviceCollection = trackingProvider.GetServiceCollection(ScopeServiceCollectionKey);

                // Only dispose explicitly registered services that were actually resolved
                foreach (var serviceDescriptor in serviceCollection)
                {
                    if (serviceDescriptor.ServiceType.IsGenericTypeDefinition)
                    {
                        continue;
                    }

                    if (trackingProvider.ResolvedServices.TryGetValue(serviceDescriptor.ServiceType, out var service) &&
                        service is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
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
