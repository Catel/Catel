namespace Catel.MVVM
{
    using Catel.IoC;
    using Logging;
    using System.Windows;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;

    /// <summary>
    /// Wrapper class to support key down events and automatically invoke commands on the <see cref="ICommandManager" />.
    /// </summary>
    public class CommandManagerWrapper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ICommandManager _commandManager;

        private bool _subscribed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManagerWrapper" /> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="commandManager">The command manager.</param>
        public CommandManagerWrapper(FrameworkElement view, ICommandManager commandManager = null)
        {
            Argument.IsNotNull("view", view);

            _commandManager = commandManager ?? ServiceLocator.Default.ResolveType<ICommandManager>();

            View = view;

            if (this.SubscribeToWeakGenericEvent<RoutedEventArgs>(view, nameof(FrameworkElement.Loaded), OnViewLoaded, false) is null)
            {
                Log.Debug("Failed to use weak events to subscribe to 'view.Loaded', going to subscribe without weak events");

                view.Loaded += OnViewLoaded;
            }

            if (this.SubscribeToWeakGenericEvent<RoutedEventArgs>(view, nameof(FrameworkElement.Unloaded), OnViewUnloaded, false) is null)
            {
                Log.Debug("Failed to use weak events to subscribe to 'view.Unloaded', going to subscribe without weak events");

                view.Unloaded += OnViewUnloaded;
            }

            Subscribe();
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>The view.</value>
        protected FrameworkElement View { get; private set; }

        private void Subscribe()
        {
            if (_subscribed)
            {
                return;
            }

            View.PreviewKeyDown += OnKeyDown;

            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed)
            {
                return;
            }

            View.PreviewKeyDown -= OnKeyDown;

            _subscribed = false;
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            Subscribe();
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            Unsubscribe();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (_commandManager.IsKeyboardEventsSuspended)
            {
                return;
            }

            if (e.Handled)
            {
                // Don't get in the way of already handled KeyDown events
                return;
            }

            // TODO: consider caching or something like that
            var commandNames = _commandManager.GetCommands();

            foreach (var commandName in commandNames)
            {
                var inputGesture = _commandManager.GetInputGesture(commandName);
                if (inputGesture is not null)
                {
                    if (inputGesture.Matches(e))
                    {
                        e.Handled = true;
                        _commandManager.ExecuteCommand(commandName);
                        break;
                    }
                }
            }
        }
    }
}
