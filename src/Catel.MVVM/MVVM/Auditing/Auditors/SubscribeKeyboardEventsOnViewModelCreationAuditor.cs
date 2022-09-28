namespace Catel.MVVM.Auditing
{
    using Catel.Services;

    public class SubscribeKeyboardEventsOnViewModelCreationAuditor : AuditorBase
    {
        private readonly ICommandManager _commandManager;
        private readonly IDispatcherService _dispatcherService;

        public SubscribeKeyboardEventsOnViewModelCreationAuditor(ICommandManager commandManager, IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(nameof(commandManager), commandManager);
            Argument.IsNotNull(nameof(dispatcherService), dispatcherService);

            _commandManager = commandManager;
            _dispatcherService = dispatcherService;
        }

        public override void OnViewModelCreated(IViewModel viewModel)
        {
            base.OnViewModelCreated(viewModel);

            _dispatcherService.BeginInvokeIfRequired(() =>
            {
                _commandManager.SubscribeToKeyboardEvents();
            });
        }
    }
}
