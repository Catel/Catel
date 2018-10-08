#if !XAMARIN

namespace Catel.MVVM.Auditing
{
    using System;
    using Catel.MVVM.Auditing;

#if UWP
    using global::Windows.UI.Xaml;
    using DispatcherTimerEventArgs = System.Object;
#else
    using System.Windows.Threading;
    using DispatcherTimerEventArgs = System.EventArgs;
#endif

    public class InvalidateCommandManagerOnViewModelInitializationAuditor : AuditorBase
    {
        private readonly ICommandManager _commandManager;
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        public InvalidateCommandManagerOnViewModelInitializationAuditor(ICommandManager commandManager)
        {
            Argument.IsNotNull(() => commandManager);

            _commandManager = commandManager;

            _dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(25)
            };

            _dispatcherTimer.Tick += OnDispatcherTimerTick;
        }

        public override void OnViewModelInitialized(IViewModel viewModel)
        {
            base.OnViewModelInitialized(viewModel);

            if (_dispatcherTimer.IsEnabled)
            {
                _dispatcherTimer.Stop();
            }

            _dispatcherTimer.Start();
        }

        private void OnDispatcherTimerTick(object sender, DispatcherTimerEventArgs e)
        {
            _dispatcherTimer.Stop();

            _commandManager.InvalidateCommands();
        }
    }
}

#endif
