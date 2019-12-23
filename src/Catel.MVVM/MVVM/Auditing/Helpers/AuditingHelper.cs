// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditingHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Auditing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Catel.Data;
    using Catel.IoC;
    using Reflection;
    using Threading;

    /// <summary>
    /// Helper for auditing which handles the complete subscription of an <see cref="IViewModel"/> instance
    /// to the current auditing manager.
    /// </summary>
    public static class AuditingHelper
    {
        private static readonly HashSet<string> KnownIgnoredPropertyNames = new HashSet<string>();
        private static readonly IObjectAdapter ObjectAdapter = ServiceLocator.Default.ResolveType<IObjectAdapter>();

        /// <summary>
        /// Initializes static members of the <see cref="AuditingHelper"/> class.
        /// </summary>
        static AuditingHelper()
        {
            KnownIgnoredPropertyNames.Add("IDataWarningInfo.Warning");
            KnownIgnoredPropertyNames.Add("INotifyDataWarningInfo.HasWarnings");
            KnownIgnoredPropertyNames.Add("IDataErrorInfo.Error");
            KnownIgnoredPropertyNames.Add("INotifyDataErrorInfo.HasErrors");
        }

        /// <summary>
        /// Registers the view model to the <see cref="AuditingManager"/>.
        /// <para />
        /// This helper will automatically unsubscribe from all events when the view model is closed.
        /// </summary>
        /// <param name="viewModel">The view model to register.</param>
        /// <remarks>
        /// This helper will call the <see cref="AuditingManager.OnViewModelCreating"/> and <see cref="AuditingManager.OnViewModelCreated"/>
        /// automatically.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        public static void RegisterViewModel(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var isAuditingEnabled = AuditingManager.IsAuditingEnabled;
            if (isAuditingEnabled)
            {
                AuditingManager.OnViewModelCreating(viewModel.GetType());
            }

            SubscribeEvents(viewModel);

            if (isAuditingEnabled)
            {
                AuditingManager.OnViewModelCreated(viewModel);
            }
        }

        /// <summary>
        /// Subscribes to all events of the view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        private static void SubscribeEvents(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            viewModel.PropertyChanged += OnViewModelPropertyChanged;
            viewModel.CommandExecutedAsync += OnViewModelCommandExecutedAsync;
            viewModel.InitializedAsync += OnViewModelInitializedAsync;
            viewModel.SavingAsync += OnViewModelSavingAsync;
            viewModel.SavedAsync += OnViewModelSavedAsync;
            viewModel.CancelingAsync += OnViewModelCancelingAsync;
            viewModel.CanceledAsync += OnViewModelCanceledAsync;
            viewModel.ClosingAsync += OnViewModelClosingAsync;
            viewModel.ClosedAsync += OnViewModelClosedAsync;
        }

        /// <summary>
        /// Unsubscribes from all events of the view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        private static void UnsubscribeEvents(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            viewModel.CommandExecutedAsync -= OnViewModelCommandExecutedAsync;
            viewModel.InitializedAsync -= OnViewModelInitializedAsync;
            viewModel.SavingAsync -= OnViewModelSavingAsync;
            viewModel.SavedAsync -= OnViewModelSavedAsync;
            viewModel.CancelingAsync -= OnViewModelCancelingAsync;
            viewModel.CanceledAsync -= OnViewModelCanceledAsync;
            viewModel.ClosingAsync -= OnViewModelClosingAsync;
            viewModel.ClosedAsync -= OnViewModelClosedAsync;
        }

        private static void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!AuditingManager.IsAuditingEnabled)
            {
                return;
            }

            var viewModel = (IViewModel)sender;

            object propertyValue = null;
            if (!string.IsNullOrEmpty(e.PropertyName) && !KnownIgnoredPropertyNames.Contains(e.PropertyName))
            {
                ObjectAdapter.GetMemberValue(viewModel, e.PropertyName, out propertyValue);
            }

            AuditingManager.OnPropertyChanged(viewModel, e.PropertyName, propertyValue);
        }

        private static Task OnViewModelCommandExecutedAsync(object sender, CommandExecutedEventArgs e)
        {
            if (!AuditingManager.IsAuditingEnabled)
            {
                return TaskHelper.Completed;
            }

            AuditingManager.OnCommandExecuted((IViewModel)sender, e.CommandPropertyName, e.Command, e.CommandParameter);

            return TaskHelper.Completed;
        }

        private static Task OnViewModelInitializedAsync(object sender, EventArgs e)
        {
            if (!AuditingManager.IsAuditingEnabled)
            {
                return TaskHelper.Completed;
            }

            AuditingManager.OnViewModelInitialized((IViewModel)sender);

            return TaskHelper.Completed;
        }

        private static Task OnViewModelSavingAsync(object sender, SavingEventArgs e)
        {
            if (!AuditingManager.IsAuditingEnabled)
            {
                return TaskHelper.Completed;
            }

            AuditingManager.OnViewModelSaving((IViewModel)sender);

            return TaskHelper.Completed;
        }

        private static Task OnViewModelSavedAsync(object sender, EventArgs e)
        {
            if (!AuditingManager.IsAuditingEnabled)
            {
                return TaskHelper.Completed;
            }

            AuditingManager.OnViewModelSaved((IViewModel)sender);

            return TaskHelper.Completed;
        }

        private static Task OnViewModelCancelingAsync(object sender, CancelingEventArgs e)
        {
            if (!AuditingManager.IsAuditingEnabled)
            {
                return TaskHelper.Completed;
            }

            AuditingManager.OnViewModelCanceling((IViewModel)sender);

            return TaskHelper.Completed;
        }

        private static Task OnViewModelCanceledAsync(object sender, EventArgs e)
        {
            if (!AuditingManager.IsAuditingEnabled)
            {
                return TaskHelper.Completed;
            }

            AuditingManager.OnViewModelCanceled((IViewModel)sender);

            return TaskHelper.Completed;
        }

        private static Task OnViewModelClosingAsync(object sender, EventArgs e)
        {
            if (!AuditingManager.IsAuditingEnabled)
            {
                return TaskHelper.Completed;
            }

            AuditingManager.OnViewModelClosing((IViewModel)sender);

            return TaskHelper.Completed;
        }

        private static Task OnViewModelClosedAsync(object sender, EventArgs e)
        {
            if (!AuditingManager.IsAuditingEnabled)
            {
                return TaskHelper.Completed;
            }

            var viewModel = (IViewModel)sender;
            AuditingManager.OnViewModelClosed(viewModel);

            UnsubscribeEvents(viewModel);

            return TaskHelper.Completed;
        }
    }
}
