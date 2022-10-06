namespace Catel.MVVM.Views
{
    using System;
    using System.Windows.Controls;

    public static partial class ViewExtensions
    {
        /// <summary>
        /// Ensures that a visual tree exists for the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public static void EnsureVisualTree(this IView view)
        {
            ArgumentNullException.ThrowIfNull(view);

            // According to the documentation, no visual tree is garantueed in the Loaded event of the user control.
            // However, as a solution the documentation says you need to manually call ApplyTemplate, so let's do that.
            // For more info, see http://msdn.microsoft.com/en-us/library/ms596558(vs.95)
            var targetControl = view as Control;
            if (targetControl is not null)
            {
                targetControl.ApplyTemplate();
            }
        }

        /// <summary>
        /// Finds the parent view model container.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The IViewModelContainer or <c>null</c> if the container is not found.</returns>
        public static IViewModelContainer? FindParentViewModelContainer(this IView view)
        {
            return FindParentByPredicate(view, o => o is IViewModelContainer) as IViewModelContainer;
        }
    }
}
