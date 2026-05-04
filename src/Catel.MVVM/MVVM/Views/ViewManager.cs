namespace Catel.MVVM.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Data;
using Logging;
using Microsoft.Extensions.Logging;

/// <summary>
/// Manager that can search for views belonging to a view model.
/// </summary>
public class ViewManager : IViewManager
{
    private readonly ILogger<ViewManager> _logger;

    /// <summary>
    /// List of views and the unique identifier of the view models they own.
    /// </summary>
    private readonly Dictionary<IView, int?> _registeredViews = new Dictionary<IView, int?>();

    private readonly object _syncObj = new object();

    public ViewManager(ILogger<ViewManager> logger)
    {
        _logger = logger;
    }

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
        ArgumentNullException.ThrowIfNull(view);

        var viewType = view.GetType().FullName;

        _logger.LogDebug("Registering view '{TypeName}'", viewType);

        lock (_syncObj)
        {
            if (_registeredViews.ContainsKey(view))
            {
                _logger.LogWarning("View '{TypeName}' is already registered", viewType);
                return;
            }

            view.ViewModelChanged += OnViewModelChanged;

            SyncViewModelOfView(view);
        }

        _logger.LogDebug("Registered view '{TypeName}'", viewType);
    }

    /// <summary>
    /// Unregisters a view so it can no longer be linked to a view model instance.
    /// </summary>
    /// <param name="view">The view to unregister.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
    public virtual void UnregisterView(IView view)
    {
        ArgumentNullException.ThrowIfNull(view);

        var viewType = view.GetType().FullName;

        _logger.LogDebug("Unregistering view '{TypeName}'", viewType);

        lock (_syncObj)
        {
            if (!_registeredViews.ContainsKey(view))
            {
                _logger.LogWarning("View '{TypeName}' is not registered", viewType);
                return;
            }

            view.ViewModelChanged -= OnViewModelChanged;

            _registeredViews.Remove(view);
        }

        _logger.LogDebug("Unregistered view '{TypeName}'", viewType);
    }

    /// <summary>
    /// Gets the views of view model.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <returns>An array containing all the views that are linked to the view.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
    public virtual IView[] GetViewsOfViewModel(IViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        _logger.LogDebug("Getting the views of view model '{UniqueIdentifier}'", BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier));

        var views = new List<IView>();

        lock (_syncObj)
        {
            views.AddRange(from registeredView in _registeredViews
                           where registeredView.Value == viewModel.UniqueIdentifier
                           select registeredView.Key);
        }

        _logger.LogDebug("Found '{Count}' views for view model '{UniqueIdentifier}'", BoxingCache.GetBoxedValue(views.Count), BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier));

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
    public IView? GetFirstOrDefaultInstance(Type viewType)
    {
        Argument.IsOfType("viewType", viewType, typeof (IView));

        return ActiveViews.FirstOrDefault(view => ObjectHelper.AreEqual(view.GetType(), viewType));
    }

    /// <summary>
    /// Called when the view model of a view has changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="eventArgs">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void OnViewModelChanged(object? sender, EventArgs eventArgs)
    {
        var view = sender as IView;
        if (view is null)
        {
            throw _logger.LogErrorAndCreateException<CatelException>($"Received ViewModelChanged event from a view without valid sender, cannot handle events correctly");
        }

        SyncViewModelOfView(view);
    }

    /// <summary>
    /// Synchronizes the view model of view.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
    private void SyncViewModelOfView(IView view)
    {
        lock (_syncObj)
        {
            var activeViewModel = view.ViewModel;
            _registeredViews[view] = (activeViewModel is not null) ? activeViewModel.UniqueIdentifier : (int?) null;
        }
    }
}
