// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandManagerBinding.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.Windows.Markup
{
    using System;
    using Catel.MVVM;
    using IoC;

#if !NETFX_CORE
    using System.Windows.Data;
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Binds commands to the command manager.
    /// </summary>
    public class CommandManagerBinding : UpdatableMarkupExtension
    {
        private readonly ICommandManager _commandManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManagerBinding"/> class.
        /// </summary>
        public CommandManagerBinding()
        {
            var dependencyResolver = this.GetDependencyResolver();
            _commandManager = dependencyResolver.Resolve<ICommandManager>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManagerBinding"/> class.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        public CommandManagerBinding(string commandName)
            : this()
        {
            CommandName = commandName;
        }

        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        /// <value>The name of the command.</value>
#if NET
        [ConstructorArgument("type")]
#endif
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

        private void OnCommandManagerCommandCreated(object sender, CommandCreatedEventArgs e)
        {
            if (string.Equals(CommandName, e.Name))
            {
                UpdateValue();
            }
        }

        /// <summary>
        /// Provides the dynamic value.
        /// </summary>
        /// <returns>System.Object.</returns>
        protected override object ProvideDynamicValue()
        {
            if (_commandManager == null)
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

#endif