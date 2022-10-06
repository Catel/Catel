namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// Extension methods for the IViewManager.
    /// </summary>
    public static class IViewManagerExtensions
    {
        /// <summary>
        /// Gets the first or default instance of the specified view type.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="viewManager">The view manager.</param>
        /// <returns>The view or <c>null</c> if the view is not registered.</returns>
        public static TView? GetFirstOrDefaultInstance<TView>(this IViewModelManager viewManager)
            where TView : IView
        {
            ArgumentNullException.ThrowIfNull(viewManager);

            var viewType = typeof(TView);

            return (TView?)viewManager.GetFirstOrDefaultInstance(viewType);
        }
    }
}
