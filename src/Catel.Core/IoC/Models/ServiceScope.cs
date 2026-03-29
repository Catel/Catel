namespace Catel.IoC;

using System;

public class ServiceScope
{
    public string? Id { get; init; }

    public required IServiceProvider ServiceProvider { get; init; }
}
