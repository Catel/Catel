// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewManagerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
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
        public static TView GetFirstOrDefaultInstance<TView>(this IViewModelManager viewManager)
            where TView : IView
        {
            Argument.IsNotNull("viewManager", viewManager);

            var viewType = typeof(TView);

            return (TView)viewManager.GetFirstOrDefaultInstance(viewType);
        }
    }
}