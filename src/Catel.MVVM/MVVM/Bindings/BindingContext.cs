// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if XAMARIN

namespace Catel.MVVM
{
    using Logging;

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Binding context that takes care of binding updates.
    /// </summary>
    public class BindingContext
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private int? _lastViewModelId;

        private readonly List<Binding> _bindings = new List<Binding>();
        private readonly List<CommandBinding> _commandBindings = new List<CommandBinding>();
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingContext"/> class.
        /// </summary>
        public BindingContext()
        {
            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<BindingContext>();

            Log.Debug("Initialized binding context");
        }

        #region Properties
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>The unique identifier.</value>
        public int UniqueIdentifier { get; private set; }

        /// <summary>
        /// Gets the get bindings.
        /// </summary>
        /// <value>The get bindings.</value>
        public IEnumerable<Binding> GetBindings
        {
            get { return _bindings.ToArray(); }
        }

        /// <summary>
        /// Gets the get command bindings.
        /// </summary>
        /// <value>The get command bindings.</value>
        public IEnumerable<CommandBinding> GetCommandBindings
        {
            get { return _commandBindings.ToArray(); }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when binding updates are required.
        /// </summary>
        public event EventHandler<EventArgs> BindingUpdateRequired;
        #endregion

        #region Methods
        /// <summary>
        /// Clears this binding context and all bindings.
        /// </summary>
        public void Clear()
        {
            Log.Debug("Clearing binding context");

            foreach (var binding in _bindings)
            {
                binding.ClearBinding();
            }

            _bindings.Clear();

            foreach (var commandBinding in _commandBindings)
            {
                commandBinding.ClearBinding();
            }

            _commandBindings.Clear();
        }

        /// <summary>
        /// Adds a new binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="binding"/> is <c>null</c>.</exception>
        public void AddBinding(Binding binding)
        {
            Argument.IsNotNull("binding", binding);

            Log.Debug("Adding binding '{0}'", binding);

            _bindings.Add(binding);
        }

        /// <summary>
        /// Removes the binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="binding"/> is <c>null</c>.</exception>
        public void RemoveBinding(Binding binding)
        {
            Argument.IsNotNull("binding", binding);

            Log.Debug("Removing binding '{0}'", binding);

            for (int i = 0; i < _bindings.Count; i++)
            {
                if (ReferenceEquals(_bindings[i], binding))
                {
                    _bindings.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Adds a new command binding.
        /// </summary>
        /// <param name="commandBinding">The command binding.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandBinding"/> is <c>null</c>.</exception>
        public void AddCommandBinding(CommandBinding commandBinding)
        {
            Argument.IsNotNull("commandBinding", commandBinding);

            Log.Debug("Adding command binding '{0}'", commandBinding);

            _commandBindings.Add(commandBinding);
        }

        /// <summary>
        /// Removes the command binding.
        /// </summary>
        /// <param name="commandBinding">The command binding.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandBinding"/> is <c>null</c>.</exception>
        public void RemoveCommandBinding(CommandBinding commandBinding)
        {
            Argument.IsNotNull("commandBinding", commandBinding);

            Log.Debug("Removing command binding '{0}'", commandBinding);

            for (int i = 0; i < _commandBindings.Count; i++)
            {
                if (ReferenceEquals(_commandBindings[i], commandBinding))
                {
                    _commandBindings.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Updates the view model of this binding context.
        /// <para />
        /// This method can be called as much as required, it will automatically determine if binding
        /// updates are required.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void DetermineIfBindingsAreRequired(IViewModel viewModel)
        {
            int? currentViewModelId = null;
            if (viewModel is not null)
            {
                currentViewModelId = viewModel.UniqueIdentifier;
            }

            if (_lastViewModelId != currentViewModelId)
            {
                if (_lastViewModelId.HasValue)
                {
                    Clear();
                }

                if (viewModel is not null)
                {
                    BindingUpdateRequired?.Invoke(this, EventArgs.Empty);
                }

                _lastViewModelId = currentViewModelId;
            }
        }
        #endregion
    }
}

#endif
