namespace Catel.MVVM.Views
{
    using Logging;

    public static partial class ViewExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the parent of the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        public static object GetParent(this IView view)
        {
            Argument.IsNotNull("view", view);

            return ((System.Windows.FrameworkElement)view).GetParent();
        }
    }
}
