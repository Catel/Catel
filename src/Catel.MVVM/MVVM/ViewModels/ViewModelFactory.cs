namespace Catel.MVVM;

using System;
using System.Linq;
using Caching;
using Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reflection;

/// <summary>
/// Default implementation of the <see cref="IViewModelFactory"/> which allows custom instantiation of view models. This way,
/// if a view model contains a complex constructor or needs caching, this factory can be used.
/// <para />
/// This default implementation will first try to inject the data context into the view model constructor. If that is not possible,
/// it will try to call the empty or default constructor.
/// </summary>
public class ViewModelFactory : IViewModelFactory
{
    private readonly ILogger<ViewModelFactory> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly ICacheStorage<Type, bool> _viewModelInjectionCache = new CacheStorage<Type, bool>();
    private readonly ICacheStorage<Type, bool> _viewModelSupportsDependencyInjectionCache = new CacheStorage<Type, bool>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelFactory" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider" /> is <c>null</c>.</exception>
    public ViewModelFactory(ILogger<ViewModelFactory> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Determines whether the specified view model is a view model with model inject. A view model is
    /// considered a model injection if the first parameter of one of the constructors is not registered inside
    /// the dependency resolver.
    /// </summary>
    /// <param name="viewModelType">Type of the view model.</param>
    /// <returns>
    ///   <c>true</c> if the view model is a view model with model injection; otherwise, <c>false</c>.
    /// </returns>
    public virtual bool IsViewModelWithModelInjection(Type viewModelType)
    {
        var isViewModelWithModelInjection = _viewModelInjectionCache.GetFromCacheOrFetch(viewModelType, () =>
        {
            var constructors = viewModelType.GetConstructorsEx();

            foreach (var constructor in constructors)
            {
                var firstParameter = constructor.GetParameters().FirstOrDefault();
                if (firstParameter is not null)
                {
                    if (!_serviceProvider.IsRegistered(firstParameter.ParameterType))
                    {
                        return true;
                    }
                }
            }

            return false;
        });

        return isViewModelWithModelInjection;
    }

    private bool CanConstructViewModelUsingDependencyInjection(Type viewModelType)
    {
        var canConstructViewModelUsingDependencyInjection = _viewModelSupportsDependencyInjectionCache.GetFromCacheOrFetch(viewModelType, () =>
        {
            var constructors = viewModelType.GetConstructorsEx();

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length == 0)
                {
                    return true;
                }

                var firstParameter = parameters[0];
                if (_serviceProvider.IsRegistered(firstParameter.ParameterType))
                {
                    return true;
                }
            }

            return false;
        });

        return canConstructViewModelUsingDependencyInjection;
    }

    /// <summary>
    /// Determines whether the specified view model as data context can be reused and allow the view to set itself as
    /// owner of the inherited view model.
    /// <para />
    /// By default a view model is allowed to be inherited when it is of the same type as the expected view model type.
    /// </summary>
    /// <param name="viewType">Type of the view.</param>
    /// <param name="expectedViewModelType">The expected view model type according to the view.</param>
    /// <param name="actualViewModelType">The actual view model type which is the type of the <paramref name="viewModelAsDataContext"/>.</param>
    /// <param name="viewModelAsDataContext">The view model as data context which must be checked.</param>
    /// <returns>
    ///   <c>true</c> if the specified view model instance ben be reused by the view; otherwise, <c>false</c>.
    /// </returns>
    public virtual bool CanReuseViewModel(Type viewType, Type expectedViewModelType, Type actualViewModelType, IViewModel? viewModelAsDataContext)
    {
        if (viewModelAsDataContext is null)
        {
            return false;
        }

        return expectedViewModelType.IsInstanceOfTypeEx(viewModelAsDataContext);
    }

    /// <summary>
    /// Determines whether the specified view model can be constructed using the injected model as a single constructor argument.
    /// This is used to distinguish between an array being passed as a single model versus an array of multiple constructor arguments.
    /// </summary>
    /// <param name="viewModelType">Type of the view model.</param>
    /// <param name="dataContextType">The type of the data context to check.</param>
    /// <returns>
    ///   <c>true</c> if the view model has at least one constructor that accepts the data context type as its first parameter; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool CanViewModelAcceptAsModel(Type viewModelType, Type dataContextType)
    {
        var constructors = viewModelType.GetConstructorsEx();

        foreach (var constructor in constructors)
        {
            var firstParameter = constructor.GetParameters().FirstOrDefault();
            if (firstParameter is not null)
            {
                if (firstParameter.ParameterType.IsAssignableFrom(dataContextType))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Creates a new view model.
    /// </summary>
    /// <param name="viewModelType">Type of the view model that needs to be created.</param>
    /// <param name="dataContext">The data context of the view model.</param>
    /// <returns>The newly created <see cref="IViewModel"/> or <c>null</c> if no view model could be created.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="viewModelType"/> does not implement the <see cref="IViewModel"/> interface.</exception>
    public virtual IViewModel? CreateViewModel(Type viewModelType, object? dataContext)
    {
        if (dataContext is null)
        {
            return CreateViewModel(viewModelType, Array.Empty<object?>());
        }

        if (dataContext is object[] args)
        {
            // If there is a constructor that accepts the array type directly (or a compatible collection type,
            // e.g. T[] -> IReadOnlyList<T>), treat the array as a single model argument instead of
            // spreading its elements as multiple constructor arguments.
            return CanViewModelAcceptAsModel(viewModelType, dataContext.GetType())
                ? CreateViewModel(viewModelType, new object?[] { dataContext })
                : CreateViewModel(viewModelType, args);
        }

        return CreateViewModel(viewModelType, new object?[] { dataContext });
    }

    /// <summary>
    /// Creates a new view model.
    /// </summary>
    /// <param name="viewModelType">Type of the view model that needs to be created.</param>
    /// <param name="args">The arguments to pass to the view model.</param>
    /// <returns>The newly created <see cref="IViewModel" /> or <c>null</c> if no view model could be created.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="viewModelType" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="viewModelType" /> does not implement the <see cref="IViewModel" /> interface.</exception>
    public virtual IViewModel? CreateViewModel(Type viewModelType, params object?[] args)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);
        Argument.ImplementsInterface("viewModelType", viewModelType, typeof(IViewModel));

        IViewModel? viewModel = null;

        // If this is a parent vm, we can skip the dependency injection
        if (args.Length == 1)
        {
            var parentViewModel = args[0] as IViewModel;
            if (parentViewModel is not null)
            {
                args = Array.Empty<object?>();
            }
        }

        // Only try to construct the view model when the injection object is not null, otherwise every
        // view model can be constructed with a nullable object. If a user wants a view model to be constructed
        // without any datacontext or injection, he/she should use an empty default constructor which will only
        // be used when injection is not possible
        if (args.Length > 0)
        {
            try
            {
               viewModel = ActivatorUtilities.CreateInstance(_serviceProvider, viewModelType, (object[])args) as IViewModel;
            }
            catch (Exception)
            {
                // Ignore since this probably does not support injection
            }

            if (viewModel is not null)
            {
                _logger.LogDebug("Constructed view model '{TypeName}' using injection of data context", viewModelType.FullName);
                return viewModel;
            }
        }

        if (!CanConstructViewModelUsingDependencyInjection(viewModelType))
        {
            _logger.LogDebug("Skipping dependency injection construction for view model '{TypeName}' because it requires argument injection", viewModelType.FullName);
            return null;
        }

        try
        {
            // Try to construct view model using dependency injection
            viewModel = ActivatorUtilities.CreateInstance(_serviceProvider, viewModelType) as IViewModel;
            if (viewModel is not null)
            {
                _logger.LogDebug("Constructed view model '{TypeName}' using dependency injection or empty constructor", viewModelType.FullName);
                return viewModel;
            }
        }
#if DEBUG
        catch (Exception ex)
#else
        catch (Exception)
#endif
        {
#if DEBUG
            _logger.LogDebug(ex, "Failed to create view model");
#endif
            // ignore
        }

        _logger.LogDebug("Could not construct view model '{TypeName}' using injection of data context'", viewModelType.FullName);

        return viewModel;
    }
}
