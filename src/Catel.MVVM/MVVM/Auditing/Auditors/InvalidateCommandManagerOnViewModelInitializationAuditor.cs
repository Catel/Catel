#if !XAMARIN

namespace Catel.MVVM.Auditing
{
    using System;
    using System.Threading;
    using Catel.MVVM.Auditing;
    using Catel.Services;

    public class InvalidateCommandManagerOnViewModelInitializationAuditor : AuditorBase
    {
        private static readonly TimeSpan TimerDuration = TimeSpan.FromMilliseconds(50);

        private readonly ICommandManager _commandManager;
        private readonly IDispatcherService _dispatcherService;

        private readonly Timer _timer;

        public InvalidateCommandManagerOnViewModelInitializationAuditor(ICommandManager commandManager,
            IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(nameof(commandManager), commandManager);
            Argument.IsNotNull(nameof(dispatcherService), dispatcherService);

            _commandManager = commandManager;
            _dispatcherService = dispatcherService;

            _timer = new Timer(OnTimerTick, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public override void OnViewModelInitialized(IViewModel viewModel)
        {
            base.OnViewModelInitialized(viewModel);

            // Reset timer
            _timer.Change(TimerDuration, Timeout.InfiniteTimeSpan);
        }

        private void OnTimerTick(object e)
        {
            _dispatcherService.BeginInvokeIfRequired(() =>
            {
                _commandManager.InvalidateCommands();
            });
        }
    }
}

#endif
