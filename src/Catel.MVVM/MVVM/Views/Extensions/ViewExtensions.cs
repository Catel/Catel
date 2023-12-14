namespace Catel.MVVM.Views
{
    using System;
    using Logging;

    public static partial class ViewExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

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
