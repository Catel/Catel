namespace Catel.MVVM.Views
{
    using System;

    public static partial class ViewExtensions
    {
        /// <summary>
        /// Gets the parent of the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        public static object? GetParent(this IView view)
        {
            ArgumentNullException.ThrowIfNull(view);

            return ((System.Windows.FrameworkElement)view).GetParent();
        }
    }
}
