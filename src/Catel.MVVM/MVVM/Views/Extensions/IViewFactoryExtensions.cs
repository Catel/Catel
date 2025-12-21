namespace Catel.MVVM.Views
{
    using System;
    using System.Windows;

    public static class IViewFactoryExtensions
    {
        /// <summary>
        /// Constructs the view with the view model. First, this method tries to inject the specified DataContext into the
        /// view. If the view does not contain a constructor with this parameter type, it will try to use the default constructor
        /// and set the DataContext manually.
        /// </summary>
        /// <typeparam name="T">The type of the view to return.</typeparam>
        /// <param name="viewFactory">The view factory.</param>
        /// <param name="viewType">Type of the view to instantiate.</param>
        /// <param name="dataContext">The data context to inject into the view. In most cases, this will be a view model.</param>
        /// <returns>
        /// The constructed view or <c>null</c> if it was not possible to construct the view.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType" /> is <c>null</c>.</exception>
        /// <remarks>
        /// Internally uses the <see cref="ConstructViewWithViewModel" /> method and casts the result.
        /// </remarks>
        public static T? ConstructViewWithViewModel<T>(this IViewFactory viewFactory, Type viewType, object? dataContext)
            where T : FrameworkElement
        {
            return viewFactory.ConstructViewWithViewModel(viewType, dataContext) as T;
        }
    }
}
