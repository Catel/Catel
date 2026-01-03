namespace Catel.MVVM.Views
{
    using System;
    using System.Windows;
    using Catel.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class ViewFactory : IViewFactory
    {
        private readonly ILogger<ViewFactory> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ViewFactory(ILogger<ViewFactory> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public virtual FrameworkElement? CreateView(Type viewType)
        {
            ArgumentNullException.ThrowIfNull(viewType);

            _logger.LogDebug("Constructing view for view type '{0}'", viewType.Name);

            FrameworkElement? view = null;

            try
            {
                view = ActivatorUtilities.CreateInstance(_serviceProvider, viewType) as FrameworkElement;
            }
            catch (Exception ex)
            {
                throw _logger.LogErrorAndCreateException<InvalidOperationException>(ex, "Failed to construct view '{0}' using default constructor", viewType.Name);
            }

            _logger.LogDebug("Constructed view using default constructor");

            return view;
        }

        /// <summary>
        /// Constructs the view with the view model. First, this method tries to inject the specified DataContext into the
        /// view. If the view does not contain a constructor with this parameter type, it will try to use the default constructor
        /// and set the DataContext manually.
        /// </summary>
        /// <param name="viewType">Type of the view to instantiate.</param>
        /// <param name="dataContext">The data context to inject into the view. In most cases, this will be a view model.</param>
        /// <returns>
        /// The constructed view or <c>null</c> if it was not possible to construct the view.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType" /> is <c>null</c>.</exception>
        public virtual FrameworkElement? CreateViewWithViewModel(Type viewType, object? dataContext)
        {
            ArgumentNullException.ThrowIfNull(viewType);

            _logger.LogDebug("Constructing view for view type '{0}'", viewType.Name);

            FrameworkElement? view = null;

            // First, try to constructor directly with the data context
            if (dataContext is not null)
            {
                try
                {
                    view = ActivatorUtilities.CreateInstance(_serviceProvider, viewType, dataContext) as FrameworkElement;
                }
                catch (Exception)
                {
                    // ignore
                }

                if (view is not null)
                {
                    _logger.LogDebug("Constructed view using injection constructor");

                    return view;
                }
            }

            _logger.LogDebug("No constructor with data (of type '{0}') injection found, trying default constructor", ObjectToStringHelper.ToTypeString(dataContext));

            try
            {
                view = ActivatorUtilities.CreateInstance(_serviceProvider, viewType) as FrameworkElement;
            }
            catch (Exception ex)
            {
                throw _logger.LogErrorAndCreateException<InvalidOperationException>(ex, "Failed to construct view '{0}' with both injection and empty constructor", viewType.Name);
            }

            view!.DataContext = dataContext;

            _logger.LogDebug("Constructed view using default constructor and setting DataContext afterwards");

            return view;
        }
    }
}
