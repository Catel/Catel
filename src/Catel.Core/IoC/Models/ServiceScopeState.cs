namespace Catel.IoC;

public class ServiceScopeState
{
    public ServiceScopeState()
    {
        
    }

    public required ServiceScope Scope { get; init; }

    public required ServiceScopeContext ScopeContext { get; init; }
}
