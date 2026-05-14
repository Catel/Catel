namespace Catel.IoC;

using System;

public class InitializeAtStartup : IInitializeAtStartup
{
    private readonly Action _action;

    public InitializeAtStartup(Action action)
    {
        _action = action;
    }

    public void Initialize()
    {
        _action();
    }
}
