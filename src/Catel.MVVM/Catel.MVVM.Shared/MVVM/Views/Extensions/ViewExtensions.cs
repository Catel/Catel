// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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

#if XAMARIN || XAMARIN_FORMS
            var userControl = view as IUserControl;
            if (userControl == null)
            {
                return null;
            }

            return userControl.Parent;
#elif NETFX_CORE
            return ((global::Windows.UI.Xaml.FrameworkElement)view).GetParent();
#else
            return ((System.Windows.FrameworkElement)view).GetParent();
#endif
        }
    }
}