namespace Catel.MVVM.Views
{
    using System;
    using System.Windows;
    using Catel.Logging;
    using Catel.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    public class ViewFactory : IViewFactory
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IServiceProvider _serviceProvider;

        public ViewFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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

            Log.Debug("Constructing view for view type '{0}'", viewType.Name);

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
                    Log.Debug("Constructed view using injection constructor");

                    return view;
                }
            }

            Log.Debug("No constructor with data (of type '{0}') injection found, trying default constructor", ObjectToStringHelper.ToTypeString(dataContext));

            try
            {
                view = ActivatorUtilities.CreateInstance(_serviceProvider, viewType) as FrameworkElement;
            }
            catch (Exception ex)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>(ex, "Failed to construct view '{0}' with both injection and empty constructor", viewType.Name);
            }

            view!.DataContext = dataContext;

            Log.Debug("Constructed view using default constructor and setting DataContext afterwards");

            return view;
        }
    }
}
