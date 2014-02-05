// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandManagerBinding.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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
    public class CommandManagerBinding : MarkupExtension
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
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
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