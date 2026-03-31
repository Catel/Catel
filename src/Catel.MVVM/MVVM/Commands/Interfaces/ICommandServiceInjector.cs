namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Internal interface for deferred service injection into commands.
    /// Services are injected when commands are attached to a ViewModel or created via a factory,
    /// allowing commands to have clean parameterless constructors.
    /// </summary>
    internal interface ICommandServiceInjector
    {
        void InjectServices(IServiceProvider serviceProvider);
    }
}
