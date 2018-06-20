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
    using System.Threading.Tasks;
    using Logging;
    using Reflection;
    using Threading;

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
            lock (_lock)
            {
                while (_viewModelInstances.Count > 0)
                {
                    RemoveViewModelInstance(_viewModelInstances[_viewModelInstances.Keys.First()]);
                }
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
                throw Log.ErrorAndCreateException(msg => new WrongViewModelTypeException(viewModel.GetType(), ViewModelType),
                    "Cannot use view model type '{0}', expected type '{1}'", viewModel.GetType().GetSafeFullName(false), ViewModelType.GetSafeFullName(false));
            }

            lock (_lock)
            {
                var vmId = viewModel.UniqueIdentifier;
                if (!_viewModelInstances.ContainsKey(vmId))
                {
                    _viewModelInstances.Add(vmId, viewModel);

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
                var vmId = viewModel.UniqueIdentifier;
                if (_viewModelInstances.Remove(vmId))
                {
                    Log.Debug("Removed view model instance, currently containing '{0}' instances of type '{1}'", _viewModelInstances.Count, ViewModelType);
                }
            }
        }
        #endregion
    }
}
