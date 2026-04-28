namespace Catel.IoC;

using Microsoft.Extensions.DependencyInjection;

public interface IScopeServiceCollectionProvider
{
    IServiceCollection AddServices(IServiceCollection serviceCollection, string? scopeId);
}
