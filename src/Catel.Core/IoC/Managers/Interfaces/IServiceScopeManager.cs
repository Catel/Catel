namespace Catel.IoC;

public interface IServiceScopeManager
{
    ServiceScope AddScope(ServiceScopeContext scopeContext);
    ServiceScope? GetScope(string id);
    bool RemoveScope(string id);
}
