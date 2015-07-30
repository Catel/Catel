// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;

    /// <summary>
    /// Manager that can search for views belonging to a view model.
    /// </summary>
    public class ViewManager : IViewManager
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        /// <summary>
        /// List of views and the unique identifyer of the view models they own.
        /// </summary>
        private readonly Dictionary<IView, int?> _registeredViews = new Dictionary<IView, int?>();

        protected readonly object _syncObj = new object();
        #endregion

        #region IViewManager Members
        /// <summary>
        /// Gets the active views presently registered.
        /// </summary>
        public IEnumerable<IView> ActiveViews
        {
            get
            {
                lock (_syncObj)
                {
                    return _registeredViews.Select(row => row.Key);
                }
            }
        }

        /// <summary>
        /// Registers a view so it can be linked to a view model instance.
        /// </summary>
        /// <param name="view">The view to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        public virtual void RegisterView(IView view)
        {
            Argument.IsNotNull("view", view);

            var viewType = view.GetType().FullName;

            Log.Debug("Registering view '{0}'", viewType);

            lock (_syncObj)
            {
                if (_registeredViews.ContainsKey(view))
                {
                    Log.Warning("View '{0}' is already registered", viewType);
                    return;
                }

                view.ViewModelChanged += OnViewModelChanged;

                SyncViewModelOfView(view);
            }

            Log.Debug("Registered view '{0}'", viewType);
        }

        /// <summary>
        /// Unregisters a view so it can no longer be linked to a view model instance.
        /// </summary>
        /// <param name="view">The view to unregister.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        public virtual void UnregisterView(IView view)
        {
            Argument.IsNotNull("view", view);

            var viewType = view.GetType().FullName;

            Log.Debug("Unregistering view '{0}'", viewType);

            lock (_syncObj)
            {
                if (!_registeredViews.ContainsKey(view))
                {
                    Log.Warning("View '{0}' is not registered", viewType);
                    return;
                }

                view.ViewModelChanged -= OnViewModelChanged;

                _registeredViews.Remove(view);
            }

            Log.Debug("Unregistered view '{0}'", viewType);
        }

        /// <summary>
        /// Gets the views of view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>An array containing all the views that are linked to the view.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public virtual IView[] GetViewsOfViewModel(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            Log.Debug("Getting the views of view model '{0}'", viewModel.UniqueIdentifier);

            var views = new List<IView>();

            lock (_syncObj)
            {
                views.AddRange(from registeredView in _registeredViews
                               where registeredView.Value == viewModel.UniqueIdentifier
                               select registeredView.Key);
            }

            Log.Debug("Found '{0}' views for view model '{1}'", views.Count, viewModel.UniqueIdentifier);

            return views.ToArray();
        }

        /// <summary>
        /// Gets the first or default instance of the specified view type.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <returns>
        /// The <see cref="IViewModel" /> or <c>null</c> if the view model is not registered.
        /// </returns>
        /// <exception cref="System.ArgumentException">The <paramref name="viewType"/> is not of type <see cref="IView"/>.</exception>
        public IView GetFirstOrDefaultInstance(Type viewType)
        {
            Argument.IsOfType("viewType", viewType, typeof (IView));

            return ActiveViews.FirstOrDefault(view => ObjectHelper.AreEqual(view.GetType(), viewType));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when the view model of a view has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnViewModelChanged(object sender, EventArgs eventArgs)
        {
            var view = ((IView) sender);
            SyncViewModelOfView(view);
        }

        /// <summary>
        /// Synchronizes the view model of view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        private void SyncViewModelOfView(IView view)
        {
            Argument.IsNotNull("view", view);

            lock (_syncObj)
            {
                var activeViewModel = view.ViewModel;
                _registeredViews[view] = (activeViewModel != null) ? activeViewModel.UniqueIdentifier : (int?) null;
            }
        }
        #endregion
    }
}