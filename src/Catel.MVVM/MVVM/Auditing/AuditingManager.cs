// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditingManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Auditing
{
    using System;
    using System.Collections.Generic;
    using IoC;

    /// <summary>
    /// Handles the auditing for MVVM inside Catel.
    /// <para/>
    /// Use this manager to register custom auditors.
    /// </summary>
    public class AuditingManager
    {
        #region Fields
        /// <summary>
        /// Instance of this singleton class.
        /// </summary>
        private static readonly AuditingManager _instance = new AuditingManager();

        /// <summary>
        /// List of currently registered auditors.
        /// </summary>
        private readonly List<IAuditor> _auditors = new List<IAuditor>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether auditing is enabled. Auditing is enabled when at least 1 auditor is registered.
        /// </summary>
        /// <value><c>true</c> if auditing is enabled; otherwise, <c>false</c>.</value>
        public static bool IsAuditingEnabled { get; private set; }

        /// <summary>
        /// Gets the number of registered auditors.
        /// </summary>
        /// <value>The number of registered auditors.</value>
        public static int RegisteredAuditorsCount
        {
            get { return _instance._auditors.Count; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Clears all the current auditors.
        /// </summary>
        public static void Clear()
        {
            lock (_instance._auditors)
            {
                _instance._auditors.Clear();

                UpdateState();
            }
        }

        /// <summary>
        /// Registers a auditor and automatically instantiates it by using the <see cref="ITypeFactory"/>.
        /// </summary>
        /// <typeparam name="TAuditor">The type of the auditor.</typeparam>
        public static void RegisterAuditor<TAuditor>()
            where TAuditor : class, IAuditor
        {
            var typeFactory = IoCConfiguration.DefaultTypeFactory;
            var auditor = typeFactory.CreateInstance<TAuditor>();

            RegisterAuditor(auditor);
        }

        /// <summary>
        /// Registers a specific auditor.
        /// </summary>
        /// <param name="auditor">The auditor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="auditor"/> is <c>null</c>.</exception>
        public static void RegisterAuditor(IAuditor auditor)
        {
            Argument.IsNotNull("auditor", auditor);

            lock (_instance._auditors)
            {
                if (!_instance._auditors.Contains(auditor))
                {
                    _instance._auditors.Add(auditor);
                }

                UpdateState();
            }
        }

        /// <summary>
        /// Unregisters a specific auditor.
        /// <para />
        /// If the auditor is not registered, nothing happens.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="auditor"/> is <c>null</c>.</exception>
        public static void UnregisterAuditor(IAuditor auditor)
        {
            Argument.IsNotNull("auditor", auditor);

            lock (_instance._auditors)
            {
                _instance._auditors.Remove(auditor);

                UpdateState();
            }
        }

        /// <summary>
        /// Must be called when a specific view model type is being created.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        internal static void OnViewModelCreating(Type viewModelType)
        {
            lock (_instance._auditors)
            {
                foreach (var auditor in _instance._auditors)
                {
                    auditor.OnViewModelCreating(viewModelType);
                }
            }
        }

        /// <summary>
        /// Must be called when a specific view model type is created.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        internal static void OnViewModelCreated(IViewModel viewModel)
        {
            lock (_instance._auditors)
            {
                foreach (var auditor in _instance._auditors)
                {
                    auditor.OnViewModelCreated(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a specific view model type is initialized.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        internal static void OnViewModelInitialized(IViewModel viewModel)
        {
            lock (_instance._auditors)
            {
                foreach (var auditor in _instance._auditors)
                {
                    auditor.OnViewModelInitialized(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when the property of a view model has just changed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newValue">The new property value.</param>
        internal static void OnPropertyChanged(IViewModel viewModel, string propertyName, object newValue)
        {
            lock (_instance._auditors)
            {
                foreach (var auditor in _instance._auditors)
                {
                    if (auditor.PropertiesToIgnore != null)
                    {
                        if (auditor.PropertiesToIgnore.Contains(propertyName))
                        {
                            continue;
                        }
                    }

                    auditor.OnPropertyChanged(viewModel, propertyName, newValue);
                }
            }
        }

        /// <summary>
        /// Must be called when a command of a view model has just been executed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="commandName">Name of the command, which is the name of the command property.</param>
        /// <param name="command">The command that has been executed.</param>
        /// <param name="commandParameter">The command parameter.</param>
        internal static void OnCommandExecuted(IViewModel viewModel, string commandName, ICatelCommand command, object commandParameter)
        {
            lock (_instance._auditors)
            {
                foreach (var auditor in _instance._auditors)
                {
                    auditor.OnCommandExecuted(viewModel, commandName, command, commandParameter);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model is about to be saved.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        internal static void OnViewModelSaving(IViewModel viewModel)
        {
            lock (_instance._auditors)
            {
                foreach (var auditor in _instance._auditors)
                {
                    auditor.OnViewModelSaving(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model has just been saved.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        internal static void OnViewModelSaved(IViewModel viewModel)
        {
            lock (_instance._auditors)
            {
                foreach (var auditor in _instance._auditors)
                {
                    auditor.OnViewModelSaved(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model is about to be canceled.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        internal static void OnViewModelCanceling(IViewModel viewModel)
        {
            lock (_instance._auditors)
            {
                foreach (var auditor in _instance._auditors)
                {
                    auditor.OnViewModelCanceling(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model has just been canceled.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        internal static void OnViewModelCanceled(IViewModel viewModel)
        {
            lock (_instance._auditors)
            {
                foreach (var auditor in _instance._auditors)
                {
                    auditor.OnViewModelCanceled(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model is about to be closed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        internal static void OnViewModelClosing(IViewModel viewModel)
        {
            lock (_instance._auditors)
            {
                foreach (var auditor in _instance._auditors)
                {
                    auditor.OnViewModelClosing(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model has just been closed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        internal static void OnViewModelClosed(IViewModel viewModel)
        {
            lock (_instance._auditors)
            {
                foreach (var auditor in _instance._auditors)
                {
                    auditor.OnViewModelClosed(viewModel);
                }
            }
        }

        /// <summary>
        /// Updates the state.
        /// </summary>
        private static void UpdateState()
        {
            lock (_instance._auditors)
            {
                IsAuditingEnabled = _instance._auditors.Count > 0;
            }
        }
        #endregion
    }
}
