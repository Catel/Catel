namespace Catel.IoC;

using System;

public class InitializeAtStartup : IInitializeAtStartup
{
    private bool _initialized;

    private readonly Action _action;

    public InitializeAtStartup(Action action)
    {
        _action = action;
    }

    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        _action();
    }
}
