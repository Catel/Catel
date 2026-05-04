namespace Catel.MVVM;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Catel.Data;
using Logging;
using Microsoft.Extensions.Logging;
using Threading;

/// <summary>
/// Manager for view models. Thanks to this manager, it is possible to subscribe to other view models and be able to respond
/// correctly to property changes in other views.
/// </summary>
public class ViewModelManager : IViewModelManager
{
    /// <summary>
    /// The lock for _instances
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    private static readonly ReaderWriterLockSlim _instancesLock;
#pragma warning restore IDE1006 // Naming Styles

    /// <summary>
    /// List of all live instances of the view model managers.
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    private static readonly List<ViewModelManager> _instances;
#pragma warning restore IDE1006 // Naming Styles

    /// <summary>
    /// The lock for the _managedViewModels dictionary.
    /// </summary>
    private readonly ReaderWriterLockSlim _managedViewModelsLock;

    /// <summary>
    /// Dictionary containing all the managed view models by this view model manager.
    /// </summary>
    private readonly Dictionary<Type, ManagedViewModel> _managedViewModels;

    /// <summary>
    /// The lock for the _viewModelModels dictionary.
    /// </summary>
    private readonly object _viewModelModelsLock = new object();

    /// <summary>
    /// Dictionary containing the unique identifiers of a all view models and their registered models.
    /// </summary>
    private readonly Dictionary<int, List<object>> _viewModelModels = new Dictionary<int, List<object>>();
    private readonly ILogger<ViewModelManager> _logger;
    private bool _disposedValue;

    /// <summary>
    /// Initializes static members of <see cref="ViewModelManager"/> class
    /// </summary>
    static ViewModelManager()
    {
        _instancesLock = new ReaderWriterLockSlim();
        _instances = new List<ViewModelManager>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelManager"/> class.
    /// </summary>
    /// <remarks>
    /// The constructor is private because this is a singleton class.
    /// </remarks>
    public ViewModelManager(ILogger<ViewModelManager> logger)
    {
        _logger = logger;

        _managedViewModelsLock = new ReaderWriterLockSlim();
        _managedViewModels = new Dictionary<Type, ManagedViewModel>();

        _instancesLock.PerformWrite(() => _instances.Add(this));
    }

    /// <summary>
    /// Gets the view model count.
    /// </summary>
    /// <value>The view model count.</value>
    public int ViewModelCount
    {
        get
        {
            return _managedViewModelsLock.PerformRead(() =>
            {
                var count = 0;
                foreach (var managedViewModel in _managedViewModels)
                {
                    if (managedViewModel.Value is not null)
                    {
                        count += managedViewModel.Value.ViewModelCount;
                    }
                }
                return count;
            });
        }
    }

    /// <summary>
    /// Gets the active view models presently registered.
    /// </summary>
    public IEnumerable<IViewModel> ActiveViewModels
    {
        get
        {
            return _managedViewModelsLock.PerformRead(() =>
            {
                return GetAllViewModels(_managedViewModels).ToList();
            });
        }
    }

    /// <summary>
    /// Registers the model of a view model.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="model">The model.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
    public void RegisterModel(IViewModel viewModel, object model)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(model);

        var viewModelTypeName = ObjectToStringHelper.ToTypeString(viewModel);
        var modelTypeName = ObjectToStringHelper.ToTypeString(model);

        _logger.LogDebug("Registering model '{ModelTypeName}' with view model '{ViewModelTypeName}' (id = '{UniqueIdentifier}')", modelTypeName, viewModelTypeName, BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier));

        lock (_viewModelModelsLock)
        {
            if (!_viewModelModels.TryGetValue(viewModel.UniqueIdentifier, out var models))
            {
                models = new List<object>();
                _viewModelModels[viewModel.UniqueIdentifier] = models;
            }

            models.Add(model);
        }

        _logger.LogDebug("Registered model '{ModelTypeName}' with view model '{ViewModelTypeName}' (id = '{UniqueIdentifier}')", modelTypeName, viewModelTypeName, BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier));
    }

    /// <summary>
    /// Unregisters the model of a view model.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="model">The model.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
    public void UnregisterModel(IViewModel viewModel, object model)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(model);

        var viewModelTypeName = ObjectToStringHelper.ToTypeString(viewModel);
        var modelTypeName = ObjectToStringHelper.ToTypeString(model);

        _logger.LogDebug("Unregistering model '{ModelTypeName}' with view model '{ViewModelTypeName}' (id = '{UniqueIdentifier}')", modelTypeName, viewModelTypeName, BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier));

        var modelWasRemoved = false;

        lock (_viewModelModelsLock)
        {
            if (_viewModelModels.TryGetValue(viewModel.UniqueIdentifier, out var models))
            {
                models.Remove(model);
                modelWasRemoved = true;
            }
        }

        if (modelWasRemoved)
        {
            _logger.LogDebug("Unregistered model '{ModelTypeName}' with view model '{ViewModelTypeName}' (id = '{UniqueIdentifier}')", modelTypeName, viewModelTypeName, BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier));
        }
        else
        {
            _logger.LogDebug("Model '{ModelTypeName}' was not registered with view model '{ViewModelTypeName}' (id = '{UniqueIdentifier}') or has already been unregistered.", modelTypeName, viewModelTypeName, BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier));
        }
    }

    /// <summary>
    /// Unregisters all models of a view model.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
    public void UnregisterAllModels(IViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var viewModelTypeName = ObjectToStringHelper.ToTypeString(viewModel);
        int modelCount = 0;

        _logger.LogDebug("Unregistering all models of view model '{ViewModelTypeName}' (id = '{UniqueIdentifier}')", viewModelTypeName, BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier));

        lock (_viewModelModelsLock)
        {
            if (_viewModelModels.TryGetValue(viewModel.UniqueIdentifier, out var models))
            {
                modelCount = models.Count;
                _viewModelModels.Remove(viewModel.UniqueIdentifier);
            }
        }

        _logger.LogDebug("Unregistered all '{ModelCount}' models of view model '{ViewModelTypeName}' (id = '{UniqueIdentifier}')", BoxingCache.GetBoxedValue(modelCount), viewModelTypeName, BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier));
    }

    /// <summary>
    /// Gets the view models of a model.
    /// </summary>
    /// <param name="model">The model to find the linked view models for.</param>
    /// <returns>An array containing all the view models.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
    public IViewModel[] GetViewModelsOfModel(object model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var modelType = ObjectToStringHelper.ToTypeString(model);

        _logger.LogDebug("Getting all view models that are linked to model '{ModelType}'", modelType);

        var viewModels = new List<IViewModel>();

        lock (_viewModelModelsLock)
        {
            foreach (var viewModelModel in _viewModelModels)
            {
                var viewModelIdentifiers = (from m in viewModelModel.Value
                                            where ObjectHelper.AreEqualReferences(m, model)
                                            select viewModelModel.Key);

                foreach (var viewModelIdentifier in viewModelIdentifiers)
                {
                    var vm = GetViewModel(viewModelIdentifier);
                    if (vm is not null)
                    {
                        viewModels.Add(vm);
                    }
                }
            }
        }

        _logger.LogDebug("Found '{Count}' view models that are linked to model '{ModelType}'", BoxingCache.GetBoxedValue(viewModels.Count), modelType);

        return viewModels.ToArray();
    }

    /// <summary>
    /// Gets the view model by its unique identifier.
    /// </summary>
    /// <param name="uniqueIdentifier">The unique identifier.</param>
    /// <returns>The <see cref="IViewModel"/> or <c>null</c> if the view model is not registered.</returns>
    public IViewModel? GetViewModel(int uniqueIdentifier)
    {
        var boxedUniqueIdentifier = BoxingCache.GetBoxedValue(uniqueIdentifier);

        _logger.LogDebug("Searching for the instance of view model with unique identifier '{UniqueIdentifier}'", boxedUniqueIdentifier);

        return _managedViewModelsLock.PerformRead(() =>
        {
            foreach (var managedViewModel in _managedViewModels)
            {
                foreach (var viewModel in managedViewModel.Value.ViewModels)
                {
                    if (viewModel.UniqueIdentifier == uniqueIdentifier)
                    {
                        _logger.LogDebug("Found the instance of view model with unique identifier '{UniqueIdentifier}' as type '{TypeName}'", boxedUniqueIdentifier, ObjectToStringHelper.ToTypeString(viewModel));

                        return viewModel;
                    }
                }
            }
            _logger.LogDebug("Did not find the instance of view model with unique identifier '{UniqueIdentifier}'. It is either not registered or not alive.", boxedUniqueIdentifier);
            return null;
        });
    }

    /// <summary>
    /// Gets the first or default instance of the specified view model.
    /// </summary>
    /// <param name="viewModelType">Type of the view mode.</param>
    /// <returns>
    /// The <see cref="IViewModel"/> or <c>null</c> if the view model is not registered.
    /// </returns>
    /// <exception cref="System.ArgumentException">The <paramref name="viewModelType"/> is not of type <see cref="IViewModel"/>.</exception>
    public IViewModel? GetFirstOrDefaultInstance(Type viewModelType)
    {
        Argument.IsOfType("viewModelType", viewModelType, typeof(IViewModel));

        return _managedViewModelsLock.PerformRead(() =>
        {
            return
                 GetAllViewModels(_managedViewModels)
                 .FirstOrDefault(viewModel => ObjectHelper.AreEqual(viewModel.GetType(), viewModelType));
        });
    }

    /// <summary>
    /// Gets the child view models of the specified view model.
    /// </summary>
    /// <param name="parentViewModel">The parent view model.</param>
    /// <returns>The child view models.</returns>
    public IEnumerable<IRelationalViewModel> GetChildViewModels(IViewModel parentViewModel)
    {
        ArgumentNullException.ThrowIfNull(parentViewModel);

        var childViewModels = GetChildViewModels(parentViewModel.UniqueIdentifier);

        return childViewModels;
    }

    /// <summary>
    /// Gets the child view models of the specified view model unique identifier.
    /// </summary>
    /// <param name="parentUniqueIdentifier">The parent unique identifier.</param>
    /// <returns>The child view models.</returns>
    public IEnumerable<IRelationalViewModel> GetChildViewModels(int parentUniqueIdentifier)
    {
        return _managedViewModelsLock.PerformRead(() =>
        {
            var relationalViewModels = GetAllViewModels(_managedViewModels).OfType<IRelationalViewModel>();

            var childViewModels = relationalViewModels.Where(viewModel => viewModel.ParentViewModel is not null && viewModel.ParentViewModel.UniqueIdentifier == parentUniqueIdentifier);

            return childViewModels.ToList();
        });
    }

    /// <summary>
    /// Registers a view model instance with the manager. All view models must register themselves to the manager.
    /// </summary>
    /// <param name="viewModel">The view model to register.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
    public void RegisterViewModelInstance(IViewModel viewModel)
    {
        RegisterViewModelInstanceInternal(viewModel);
    }

    /// <summary>
    /// Registers a view model instance with the manager. All view models must register themselves to the manager.
    /// </summary>
    /// <param name="viewModel">The view model to register.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
    private void RegisterViewModelInstanceInternal(IViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var managedViewModel = GetManagedViewModel(viewModel.GetType());
        managedViewModel.AddViewModelInstance(viewModel);
    }

    /// <summary>
    /// Unregisters a view model instance from the manager. All view models must unregister themselves from the manager.
    /// </summary>
    /// <param name="viewModel">The view model to unregister.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
    public void UnregisterViewModelInstance(IViewModel viewModel)
    {
        UnregisterViewModelInstanceInternal(viewModel);
    }

    /// <summary>
    /// Unregisters a view model instance from the manager. All view models must unregister themselves from the manager.
    /// </summary>
    /// <param name="viewModel">The view model to unregister.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
    private void UnregisterViewModelInstanceInternal(IViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var managedViewModel = GetManagedViewModel(viewModel.GetType());
        managedViewModel.RemoveViewModelInstance(viewModel);
    }

    /// <summary>
    /// Gets the active view models.
    /// </summary>
    /// <param name="managedViewModels">Dictionary of view-models</param>
    /// <returns></returns>
    private static IEnumerable<IViewModel> GetAllViewModels(Dictionary<Type, ManagedViewModel> managedViewModels)
    {
        return managedViewModels.SelectMany(row => row.Value.ViewModels).ToList();
    }

    /// <summary>
    /// Gets the managed view model for a specific view model type.
    /// </summary>
    /// <param name="viewModelType">Type of the view model.</param>
    /// <returns>The <see cref="ManagedViewModel"/> of the specified type.</returns>
    private ManagedViewModel GetManagedViewModel(Type viewModelType)
    {
        return _managedViewModelsLock.PerformUpgradableRead(() =>
        {
            if (_managedViewModels.TryGetValue(viewModelType, out var result))
            {
                return result;
            }

            result = new ManagedViewModel(viewModelType);

            _managedViewModelsLock.PerformWrite(() =>
            {
                _managedViewModels.Add(viewModelType, result);
            });

            return result;
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _managedViewModelsLock?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
