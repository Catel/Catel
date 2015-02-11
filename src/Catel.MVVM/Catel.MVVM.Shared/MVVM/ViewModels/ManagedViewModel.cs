// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagedViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Logging;

    /// <summary>
    /// Represents a managed view model. A managed view model is watched for property changes. As soon as a change occurs in one of the
    /// managed view models, all other interested view models are notified of the changes.
    /// </summary>
    internal class ManagedViewModel
    {
        #region Fields
        /// <summary>
        /// The <see cref="ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// List of alive view model instances.
        /// </summary>
        private readonly Dictionary<int, IViewModel> _viewModelInstances = new Dictionary<int, IViewModel>();

        /// <summary>
        /// List of alive view model instances that are interested in other view models.
        /// </summary>
        private readonly Dictionary<int, IViewModel> _interestedViewModels = new Dictionary<int, IViewModel>();

        /// <summary>
        /// Lock object.
        /// </summary>
        private readonly object _lock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedViewModel"/> class.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        public ManagedViewModel(Type viewModelType)
        {
            Argument.IsNotNull("viewModelType", viewModelType);

            ViewModelType = viewModelType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <value>The type of the view model.</value>
        public Type ViewModelType { get; private set; }

        /// <summary>
        /// Gets the view model count.
        /// </summary>
        /// <value>The view model count.</value>
        public int ViewModelCount
        {
            get { return _viewModelInstances.Count; }
        }

        /// <summary>
        /// Gets all the currently registered view models managed by this view model manager.
        /// </summary>
        public IViewModel[] ViewModels
        {
            get { return _viewModelInstances.Values.ToArray(); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Clears all the currently registered view models.
        /// </summary>
        /// <remarks>
        /// This method should only be called during unit testing.
        /// </remarks>
        internal void Clear()
        {
            while (_interestedViewModels.Count > 0)
            {
                RemoveInterestedViewModel(_interestedViewModels[_interestedViewModels.Keys.First()]);
            }

            while (_viewModelInstances.Count > 0)
            {
                RemoveViewModelInstance(_viewModelInstances[_viewModelInstances.Keys.First()]);
            }
        }

        /// <summary>
        /// Adds a view model instance to the list of instances.
        /// </summary>
        /// <param name="viewModel">The view model instance to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="WrongViewModelTypeException">The <paramref name="viewModel"/> is not of the right type.</exception>
        public void AddViewModelInstance(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            if (viewModel.GetType() != ViewModelType)
            {
                throw new WrongViewModelTypeException(viewModel.GetType(), ViewModelType);
            }

            lock (_lock)
            {
                var vmId = viewModel.UniqueIdentifier;
                if (!_viewModelInstances.ContainsKey(vmId))
                {
                    _viewModelInstances.Add(vmId, viewModel);

                    viewModel.PropertyChanged += OnViewModelPropertyChanged;
                    var viewModelBase = viewModel as ViewModelBase;
                    if (viewModelBase != null)
                    {
                        viewModelBase.CommandExecuted += OnViewModelCommandExecuted;
                    }

                    viewModel.Saving += OnViewModelSaving;
                    viewModel.Saved += OnViewModelSaved;
                    viewModel.Canceling += OnViewModelCanceling;
                    viewModel.Canceled += OnViewModelCanceled;
                    viewModel.Closed += OnViewModelClosed;

                    Log.Debug("Added view model instance, currently containing '{0}' instances of type '{1}'", _viewModelInstances.Count, ViewModelType);
                }
            }
        }

        /// <summary>
        /// Removes a view model instance from the list of instances.
        /// </summary>
        /// <param name="viewModel">The view model instance to remove.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public void RemoveViewModelInstance(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            lock (_lock)
            {
                viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                var viewModelBase = viewModel as ViewModelBase;
                if (viewModelBase != null)
                {
                    viewModelBase.CommandExecuted -= OnViewModelCommandExecuted;
                }
                viewModel.Saving -= OnViewModelSaving;
                viewModel.Saved -= OnViewModelSaved;
                viewModel.Canceling -= OnViewModelCanceling;
                viewModel.Canceled -= OnViewModelCanceled;
                viewModel.Closed -= OnViewModelClosed;

                var vmId = viewModel.UniqueIdentifier;
                if (_viewModelInstances.ContainsKey(vmId))
                {
                    _viewModelInstances.Remove(vmId);

                    Log.Debug("Removed view model instance, currently containing '{0}' instances of type '{1}'", _viewModelInstances.Count, ViewModelType);
                }
            }
        }

        /// <summary>
        /// Adds a view model to the list of interested view models for this view model type.
        /// </summary>
        /// <param name="viewModel">The view model instance that is interested in changes.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public void AddInterestedViewModel(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            lock (_lock)
            {
                var vmId = viewModel.UniqueIdentifier;
                _interestedViewModels.Add(vmId, viewModel);

                viewModel.Closed += OnInterestedViewModelClosed;

                Log.Debug("Added interested view model of type '{0}' for type '{1}', currently containing {2} interested view model(s)",
                    viewModel.GetType(), ViewModelType, _interestedViewModels.Count);
            }
        }

        /// <summary>
        /// Removes a view model from the list of interested view models for this view model type.
        /// </summary>
        /// <param name="viewModel">The view model instance that is interested in changes.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public void RemoveInterestedViewModel(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            lock (_lock)
            {
                viewModel.Closed -= OnInterestedViewModelClosed;

                var vmId = viewModel.UniqueIdentifier;
                if (_interestedViewModels.ContainsKey(vmId))
                {
                    _interestedViewModels.Remove(vmId);

                    Log.Debug("Removed interested view model of type '{0}' for type '{1}', currently containing {2} interested view model(s)",
                              viewModel.GetType(), ViewModelType, _interestedViewModels.Count);
                }
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the ViewModel instances.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            lock (_lock)
            {
                var viewModels = (from viewModel in _interestedViewModels
                                  select viewModel.Value);

                foreach (var viewModel in viewModels)
                {
                    try
                    {
                        var notifyableViewModel = viewModel as INotifyableViewModel;
                        if (notifyableViewModel != null)
                        {
                            notifyableViewModel.ViewModelPropertyChanged((IViewModel)sender, e.PropertyName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to let an interested view model know that a view model has changed. Probably the view model is not correctly cleaned up");
                    }
                }
            }
        }

        /// <summary>
        /// Called when the <see cref="ViewModelBase.CommandExecuted"/> event is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Catel.MVVM.CommandExecutedEventArgs"/> instance containing the event data.</param>
        private void OnViewModelCommandExecuted(object sender, CommandExecutedEventArgs e)
        {
            lock (_lock)
            {
                var viewModels = (from viewModel in _interestedViewModels
                                  select viewModel.Value);

                foreach (var viewModel in viewModels)
                {
                    try
                    {
                        var notifyableViewModel = viewModel as INotifyableViewModel;
                        if (notifyableViewModel != null)
                        {
                            notifyableViewModel.ViewModelCommandExecuted((IViewModel)sender, e.Command, e.CommandParameter);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to let an interested view model know that a view model has changed. Probably the view model is not correctly cleaned up");
                    }
                }
            }
        }

        /// <summary>
        /// Called when the <see cref="ViewModelBase.Saving"/> event is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnViewModelSaving(object sender, EventArgs e)
        {
            NotifyViewModelsOfEvent((IViewModel)sender, ViewModelEvent.Saving, e);
        }

        /// <summary>
        /// Called when the <see cref="ViewModelBase.Saved"/> event is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnViewModelSaved(object sender, EventArgs e)
        {
            NotifyViewModelsOfEvent((IViewModel)sender, ViewModelEvent.Saved, e);
        }

        /// <summary>
        /// Called when the <see cref="ViewModelBase.Canceling"/> event is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnViewModelCanceling(object sender, EventArgs e)
        {
            NotifyViewModelsOfEvent((IViewModel)sender, ViewModelEvent.Canceling, e);
        }

        /// <summary>
        /// Called when the <see cref="ViewModelBase.Canceled"/> event is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnViewModelCanceled(object sender, EventArgs e)
        {
            NotifyViewModelsOfEvent((IViewModel)sender, ViewModelEvent.Canceled, e);
        }

        /// <summary>
        /// Called when the <see cref="ViewModelBase.Canceled"/> event is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnViewModelClosed(object sender, EventArgs e)
        {
            NotifyViewModelsOfEvent((IViewModel)sender, ViewModelEvent.Closed, e);
        }

        /// <summary>
        /// Called when the <see cref="ViewModelBase.Closed"/> event is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnInterestedViewModelClosed(object sender, EventArgs e)
        {
            RemoveInterestedViewModel((IViewModel)sender);
        }

        /// <summary>
        /// Notifies all interested view models of an event that took place.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="viewModelEvent">The view model event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="e"/> is <c>null</c>.</exception>
        private void NotifyViewModelsOfEvent(IViewModel viewModel, ViewModelEvent viewModelEvent, EventArgs e)
        {
            Argument.IsNotNull("viewModel", viewModel);
            Argument.IsNotNull("e", e);

            lock (_lock)
            {
                var viewModels = (from viewModelKeyValuePair in _interestedViewModels
                                  select viewModelKeyValuePair.Value);

                foreach (var vm in viewModels)
                {
                    try
                    {
                        var notifyableViewModel = vm as INotifyableViewModel;
                        if (notifyableViewModel != null)
                        {
                            notifyableViewModel.ViewModelEvent(viewModel, viewModelEvent, e);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to let an interested view model know that a view model has changed. Probably the view model is not correctly cleaned up");
                    }
                }
            }
        }
        #endregion
    }
}
