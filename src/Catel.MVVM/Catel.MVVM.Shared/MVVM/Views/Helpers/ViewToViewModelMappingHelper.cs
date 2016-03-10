// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewToViewModelMappingHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Logging;
    using Reflection;

    /// <summary>
    /// Helper class to fix <see cref="ViewToViewModelMapping"/> for <see cref="IView"/>.
    /// </summary>
    internal class ViewToViewModelMappingHelper
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dictionary of <see cref="IViewModelContainer"/> instances managed by this helper class.
        /// </summary>
        private static readonly Dictionary<IViewModelContainer, ViewToViewModelMappingHelper> _viewModelContainers = new Dictionary<IViewModelContainer, ViewToViewModelMappingHelper>();

        /// <summary>
        /// Dictionary of <see cref="ViewToViewModelMappingContainer"/> instances per type.
        /// </summary>
        private static readonly Dictionary<Type, ViewToViewModelMappingContainer> _viewToViewModelMappingContainers = new Dictionary<Type, ViewToViewModelMappingContainer>();

        /// <summary>
        /// List of properties in the view model that should be ignored.
        /// </summary>
        private readonly HashSet<string> _ignoredViewModelChanges = new HashSet<string>();

        /// <summary>
        /// List of properties in the view that should be ignored.
        /// </summary>
        private readonly HashSet<string> _ignoredViewChanges = new HashSet<string>();

        /// <summary>
        /// Gets or sets the previous view model.
        /// </summary>
        /// <value>The previous view model.</value>
        private IViewModel _previousViewModel;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewToViewModelMappingHelper"/> class.
        /// </summary>
        /// <param name="viewModelContainer">The view model container.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelContainer"/> is <c>null</c>.</exception>
        public ViewToViewModelMappingHelper(IViewModelContainer viewModelContainer)
        {
            Argument.IsNotNull("viewModelContainer", viewModelContainer);

            Log.Debug("Initializing view model container to manage ViewToViewModel mappings");

            ViewModelContainer = viewModelContainer;
            var viewModelContainerType = ViewModelContainerType;

            if (!_viewToViewModelMappingContainers.ContainsKey(viewModelContainerType))
            {
                _viewToViewModelMappingContainers.Add(viewModelContainerType, new ViewToViewModelMappingContainer(viewModelContainer));
            }

            ViewModelContainer.ViewModelChanged += OnViewModelChanged;
            ViewModelContainer.PropertyChanged += OnViewModelContainerPropertyChanged;

            InitializeViewModel(ViewModelContainer.ViewModel);

            Log.Debug("Initialized view model container to manage ViewToViewModel mappings");
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the view model container.
        /// </summary>
        /// <value>The view model container.</value>
        public IViewModelContainer ViewModelContainer { get; private set; }

        /// <summary>
        /// Gets the type of the view model container.
        /// </summary>
        /// <value>The type of the view model container.</value>
        private Type ViewModelContainerType
        {
            get { return ViewModelContainer.GetType(); }
        }

        /// <summary>
        /// Gets the current view model.
        /// </summary>
        /// <value>The current view model.</value>
        private IViewModel CurrentViewModel
        {
            get { return ViewModelContainer.ViewModel; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the <see cref="ViewToViewModelMapping"/> for the specified <see cref="IViewModelContainer"/>.
        /// </summary>
        /// <param name="viewModelContainer">The view model container to initialize the mappings for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelContainer"/> is <c>null</c>.</exception>
        public static void InitializeViewToViewModelMappings(IViewModelContainer viewModelContainer)
        {
            Argument.IsNotNull("viewModelContainer", viewModelContainer);

            if (_viewModelContainers.ContainsKey(viewModelContainer))
            {
                return;
            }

            _viewModelContainers.Add(viewModelContainer, new ViewToViewModelMappingHelper(viewModelContainer));
        }

        /// <summary>
        /// Uninitializes the <see cref="ViewToViewModelMapping"/> for the specified <see cref="IViewModelContainer"/>.
        /// </summary>
        /// <param name="viewModelContainer">The view model container the uninitialize the mappings for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelContainer"/> is <c>null</c>.</exception>
        public static void UninitializeViewToViewModelMappings(IViewModelContainer viewModelContainer)
        {
            Argument.IsNotNull("viewModelContainer", viewModelContainer);

            if (_viewModelContainers.ContainsKey(viewModelContainer))
            {
                _viewModelContainers[viewModelContainer].UninitializeViewToViewModelMappings();
                _viewModelContainers.Remove(viewModelContainer);
            }
        }

        /// <summary>
        /// Uninitializes the <see cref="ViewToViewModelMapping"/> for the registered <see cref="IViewModelContainer"/>.
        /// </summary>
        private void UninitializeViewToViewModelMappings()
        {
            Log.Debug("Uninitializing view model container to manage ViewToViewModel mappings");

            ViewModelContainer.ViewModelChanged -= OnViewModelChanged;
            ViewModelContainer.PropertyChanged -= OnViewModelContainerPropertyChanged;

            UninitializeViewModel(CurrentViewModel);

            Log.Debug("Uninitialized view model container to manage ViewToViewModel mappings");
        }

        /// <summary>
        /// Initializes the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        private void InitializeViewModel(IViewModel viewModel)
        {
            var viewModelType = ObjectToStringHelper.ToTypeString(viewModel);

            Log.Debug("Initializing view model '{0}'", viewModelType);

            UninitializeViewModel(_previousViewModel);

            _previousViewModel = viewModel;

            if (viewModel != null)
            {
                // If there are mappings, sync them in the right way
                var viewModelContainerType = ViewModelContainerType;
                foreach (var mapping in _viewToViewModelMappingContainers[viewModelContainerType].GetAllViewToViewModelMappings())
                {
                    try
                    {
                        if ((mapping.MappingType == ViewToViewModelMappingType.TwoWayViewWins) ||
                            (mapping.MappingType == ViewToViewModelMappingType.ViewToViewModel))
                        {
                            TransferValueFromViewToViewModel(viewModel, mapping.ViewPropertyName, mapping.ViewModelPropertyName);
                        }
                        else if ((mapping.MappingType == ViewToViewModelMappingType.TwoWayViewModelWins) ||
                                 (mapping.MappingType == ViewToViewModelMappingType.ViewModelToView))
                        {
                            TransferValueFromViewModelToView(viewModel, mapping.ViewPropertyName, mapping.ViewModelPropertyName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to transfer value from view property '{0}' to the view model property '{1}' for the ViewToViewModelMapping", 
                            mapping.ViewPropertyName, mapping.ViewModelPropertyName);
                    }
                }

                viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            Log.Debug("Initialized view model '{0}'", viewModelType);
        }

        /// <summary>
        /// Uninitializes the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        private void UninitializeViewModel(IViewModel viewModel)
        {
            if (viewModel == null)
            {
                return;
            }

            var viewModelType = viewModel.GetType().Name;

            Log.Debug("Uninitializing view model '{0}'", viewModelType);

            viewModel.PropertyChanged -= OnViewModelPropertyChanged;

            Log.Debug("Uninitialized view model '{0}'", viewModelType);
        }

        /// <summary>
        /// Called when the view model on the view model container has changed.
        /// </summary>
        /// <param name="sender">The view model container.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnViewModelChanged(object sender, EventArgs e)
        {
            InitializeViewModel(CurrentViewModel);
        }

        /// <summary>
        /// Called when a property on the view model has changed.
        /// </summary>
        /// <param name="sender">The view model.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModelContainerType = ViewModelContainerType;
            if (_viewToViewModelMappingContainers[viewModelContainerType].ContainsViewModelToViewMapping(e.PropertyName))
            {
                ViewToViewModelMapping mapping = _viewToViewModelMappingContainers[viewModelContainerType].GetViewModelToViewMapping(e.PropertyName);
                if (_ignoredViewModelChanges.Contains(mapping.ViewPropertyName))
                {
                    Log.Debug("Ignored property changed event for ViewModel.'{0}'", mapping.ViewPropertyName);
                }
                else
                {
                    if ((mapping.MappingType == ViewToViewModelMappingType.TwoWayDoNothing) ||
                        (mapping.MappingType == ViewToViewModelMappingType.TwoWayViewWins) ||
                        (mapping.MappingType == ViewToViewModelMappingType.TwoWayViewModelWins) ||
                        (mapping.MappingType == ViewToViewModelMappingType.ViewModelToView))
                    {
                        TransferValueFromViewModelToView(CurrentViewModel, mapping.ViewPropertyName, mapping.ViewModelPropertyName);
                    }
                }
            }
        }

        /// <summary>
        /// Called when a property on the view model container has changed.
        /// </summary>
        /// <param name="sender">The view model container.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnViewModelContainerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModelContainerType = ViewModelContainerType;
            if (_viewToViewModelMappingContainers[viewModelContainerType].ContainsViewToViewModelMapping(e.PropertyName))
            {
                ViewToViewModelMapping mapping = _viewToViewModelMappingContainers[viewModelContainerType].GetViewToViewModelMapping(e.PropertyName);
                if (_ignoredViewChanges.Contains(mapping.ViewPropertyName))
                {
                    Log.Debug("Ignored property changed event for view.'{0}'", mapping.ViewPropertyName);
                }
                else
                {
                    if ((mapping.MappingType == ViewToViewModelMappingType.TwoWayDoNothing) ||
                        (mapping.MappingType == ViewToViewModelMappingType.TwoWayViewWins) ||
                        (mapping.MappingType == ViewToViewModelMappingType.TwoWayViewModelWins) ||
                        (mapping.MappingType == ViewToViewModelMappingType.ViewToViewModel))
                    {
                        TransferValueFromViewToViewModel(CurrentViewModel, mapping.ViewPropertyName, mapping.ViewModelPropertyName);
                    }
                }
            }
        }

        /// <summary>
        /// Transfers the value from a view property to the view model property.
        /// <para/>
        /// This method does nothing when <paramref name="viewModel"/> is <c>null</c>.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="viewPropertyName">Name of the view property.</param>
        /// <param name="viewModelPropertyName">Name of the view model property.</param>
        /// <exception cref="ArgumentException">The <paramref name="viewPropertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="viewModelPropertyName"/> is <c>null</c> or whitespace.</exception>
        /// <remarks>
        /// This method does not check the type of the properties. If the types are incorrect, an exception will be thrown by
        /// the .NET Framework.
        /// </remarks>
        protected void TransferValueFromViewToViewModel(IViewModel viewModel, string viewPropertyName, string viewModelPropertyName)
        {
            Argument.IsNotNullOrWhitespace("viewPropertyName", viewPropertyName);
            Argument.IsNotNullOrWhitespace("viewModelPropertyName", viewModelPropertyName);

            if (viewModel == null)
            {
                Log.Warning("Cannot transfer value from view to view model because view model is null");
                return;
            }

            Log.Debug("Ignore next property changed event for view model.'{0}'", viewModelPropertyName);

            // Ignore this property (we will soon receive an event that it has changed)
            if (!_ignoredViewModelChanges.Contains(viewModelPropertyName))
            {
                _ignoredViewModelChanges.Add(viewModelPropertyName);
            }

            TransferValue(ViewModelContainer, viewPropertyName, viewModel, viewModelPropertyName);

            Log.Debug("No longer ignoring next property changed event for view model.'{0}'", viewModelPropertyName);

            _ignoredViewModelChanges.Remove(viewModelPropertyName);
        }

        /// <summary>
        /// Transfers the value from a view model property to the view property.
        /// <para/>
        /// This method does nothing when <paramref name="viewModel"/> is <c>null</c>.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="viewPropertyName">Name of the view property.</param>
        /// <param name="viewModelPropertyName">Name of the view model property.</param>
        /// <exception cref="ArgumentException">The <paramref name="viewPropertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="viewModelPropertyName"/> is <c>null</c> or whitespace.</exception>
        /// <remarks>
        /// This method does not check the type of the properties. If the types are incorrect, an exception will be thrown by
        /// the .NET Framework.
        /// </remarks>
        protected void TransferValueFromViewModelToView(IViewModel viewModel, string viewPropertyName, string viewModelPropertyName)
        {
            Argument.IsNotNullOrWhitespace("viewPropertyName", viewPropertyName);
            Argument.IsNotNullOrWhitespace("viewModelPropertyName", viewModelPropertyName);

            if (viewModel == null)
            {
                Log.Warning("Cannot transfer value from view model to view because view model is null");
                return;
            }

            Log.Debug("Ignore next property changed event for view.'{0}'", viewPropertyName);

            if (!_ignoredViewChanges.Contains(viewPropertyName))
            {
                _ignoredViewChanges.Add(viewPropertyName);
            }

            TransferValue(viewModel, viewModelPropertyName, ViewModelContainer, viewPropertyName);

            Log.Debug("No longer ignoring next property changed event for view.'{0}'", viewPropertyName);

            _ignoredViewChanges.Remove(viewPropertyName);
        }

        /// <summary>
        /// Transfers a value from the source property to the target property.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        /// <param name="targetPropertyName">Name of the target property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="sourcePropertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="targetPropertyName"/> is <c>null</c> or whitespace.</exception>
        /// <remarks>
        /// This method does not check the type of the properties. If the types are incorrect, an exception will be thrown by
        /// the .NET Framework.
        /// </remarks>
        private static void TransferValue(object source, string sourcePropertyName, object target, string targetPropertyName)
        {
            Argument.IsNotNull("source", source);
            Argument.IsNotNull("target", target);
            Argument.IsNotNullOrWhitespace("sourcePropertyName", sourcePropertyName);
            Argument.IsNotNullOrWhitespace("targetPropertyName", targetPropertyName);

            object valueToTransfer = PropertyHelper.GetPropertyValue(source, sourcePropertyName, false);

            Log.Debug("Transferring value of {0}.{1} to {2}.{3}", source.GetType().Name, sourcePropertyName, target.GetType().Name, targetPropertyName);

            PropertyHelper.SetPropertyValue(target, targetPropertyName, valueToTransfer, false);
        }
        #endregion
    }
}
