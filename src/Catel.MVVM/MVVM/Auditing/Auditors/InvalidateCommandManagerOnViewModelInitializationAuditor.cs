namespace Catel.MVVM.Auditing
{
    using System;
    using System.Threading;
    using Catel.Services;

    public class InvalidateCommandManagerOnViewModelInitializationAuditor : AuditorBase
    {
        private static readonly TimeSpan TimerDuration = TimeSpan.FromMilliseconds(50);

        private readonly ICommandManager _commandManager;
        private readonly IDispatcherService _dispatcherService;

#pragma warning disable IDISP006 // Implement IDisposable.
        private readonly Timer _timer;
#pragma warning restore IDISP006 // Implement IDisposable.

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
