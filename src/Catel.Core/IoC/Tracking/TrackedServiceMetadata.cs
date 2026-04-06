namespace Catel.IoC;

public class TrackedServiceMetadata
{
    public required TrackedServiceKey Key { get; init; }

    public required object ServiceInstance { get; init; }
}
