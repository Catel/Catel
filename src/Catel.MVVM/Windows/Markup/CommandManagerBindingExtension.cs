namespace Catel.Windows.Markup
{
    using System;
    using Catel.MVVM;
    using IoC;
    using System.Windows.Markup;

    /// <summary>
    /// Binds commands to the command manager.
    /// </summary>
    public class CommandManagerBindingExtension : UpdatableMarkupExtension
    {
        private readonly ICommandManager _commandManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManagerBindingExtension"/> class.
        /// </summary>
        public CommandManagerBindingExtension()
            : this(string.Empty)
        {
            // Leave empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManagerBindingExtension"/> class.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        public CommandManagerBindingExtension(string commandName)
        {
            CommandName = commandName;

            var dependencyResolver = this.GetDependencyResolver();
            _commandManager = dependencyResolver.ResolveRequired<ICommandManager>();
        }

        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        /// <value>The name of the command.</value>
        [ConstructorArgument("commandName")]
        public string CommandName { get; set; }

        /// <summary>
        /// Called when the target object has been loaded.
        /// </summary>
        protected override void OnTargetObjectLoaded()
        {
            base.OnTargetObjectLoaded();

            _commandManager.CommandCreated += OnCommandManagerCommandCreated;

            // It's possible that we have a late-bound command, always update
            UpdateValue();
        }

        /// <summary>
        /// Called when the target object has been unloaded.
        /// </summary>
        protected override void OnTargetObjectUnloaded()
        {
            _commandManager.CommandCreated -= OnCommandManagerCommandCreated;

            base.OnTargetObjectUnloaded();
        }

        private void OnCommandManagerCommandCreated(object? sender, CommandCreatedEventArgs e)
        {
            if (string.Equals(CommandName, e.Name))
            {
                UpdateValue();
            }
        }

        /// <summary>
        /// Provides the dynamic value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>System.Object.</returns>
        protected override object? ProvideDynamicValue(IServiceProvider? serviceProvider)
        {
            if (_commandManager is null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(CommandName))
            {
                return null;
            }

            var command = _commandManager.GetCommand(CommandName);
            return command;
        }
    }
}
