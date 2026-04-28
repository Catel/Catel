namespace Catel.IoC;

using System;

public record TrackedServiceKey
{
    public required Type ServiceType { get; init; }

    public required object? ServiceKey { get; init; }
}
