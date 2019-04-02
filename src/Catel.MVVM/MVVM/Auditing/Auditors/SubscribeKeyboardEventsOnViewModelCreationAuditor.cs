#if !XAMARIN

namespace Catel.MVVM.Auditing
{
    public class SubscribeKeyboardEventsOnViewModelCreationAuditor : AuditorBase
    {
        private readonly ICommandManager _commandManager;

        public SubscribeKeyboardEventsOnViewModelCreationAuditor(ICommandManager commandManager)
        {
            Argument.IsNotNull(() => commandManager);

            _commandManager = commandManager;
        }

        public override void OnViewModelCreated(IViewModel viewModel)
        {
            base.OnViewModelCreated(viewModel);

            _commandManager.SubscribeToKeyboardEvents();
        }
    }
}

#endif
