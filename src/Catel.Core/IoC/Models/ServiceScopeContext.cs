namespace Catel.IoC;

public class ServiceScopeContext
{
    public ServiceScopeContext()
    {
        
    }

    public required string Id { get; init; }

    /// <summary>
    /// If set to <c>true</c>, the scope service provider will be disposed when the scope is removed.
    /// <para/>
    /// The default value is <c>false</c>.
    /// </summary>
    public bool DisposeServiceProvider { get; set; }

    /// <summary>
    /// If set to <c>true</c>, the services explicitly added to this scope will be disposed when the scope is removed.
    /// <para/>
    /// The default value is <c>false</c>.
    /// </summary>
    /// <remarks>
    /// Note that when using this feature, it might instantiate services during the scope removal that were not yet
    /// instantiated.
    /// </remarks>
    public bool DisposeScopeServices { get; set; }

    /// <summary>
    /// If set to <c>true</c>, the scope will be isolated from parent scopes. This means that 
    /// services registered in parent scopes will not be accessible in this scope.
    /// <para/>
    /// The default value is <c>false</c>.
    /// </summary>
    public bool IsIsolated { get; set; }
}
