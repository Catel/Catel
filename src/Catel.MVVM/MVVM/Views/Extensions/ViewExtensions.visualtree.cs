// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExtensions.visualtree.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
#if XAMARIN || XAMARIN_FORMS
    // nothing
#elif UWP
    using global::Windows.UI.Xaml.Controls;
#else
    using System.Windows.Controls;
#endif

    public static partial class ViewExtensions
    {
        #region Methods
        /// <summary>
        /// Ensures that a visual tree exists for the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public static void EnsureVisualTree(this IView view)
        {
            Argument.IsNotNull("view", view);

#if !XAMARIN && !XAMARIN_FORMS
            // According to the documentation, no visual tree is garantueed in the Loaded event of the user control.
            // However, as a solution the documentation says you need to manually call ApplyTemplate, so let's do that.
            // For more info, see http://msdn.microsoft.com/en-us/library/ms596558(vs.95)
            var targetControl = view as Control;
            if (targetControl is not null)
            {
                (targetControl).ApplyTemplate();
            }
#endif
        }

        /// <summary>
        /// Finds the parent view model container.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The IViewModelContainer or <c>null</c> if the container is not found.</returns>
        public static IViewModelContainer FindParentViewModelContainer(this IView view)
        {
#if XAMARIN_FORMS
            // TODO: Search the parent if is possible.
            return null;
#else
            return FindParentByPredicate(view, o => o is IViewModelContainer) as IViewModelContainer;
#endif
        }
#endregion
    }
}
