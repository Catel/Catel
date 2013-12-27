using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catel.MVVM.Views.Interfaces
{
    using Windows.Controls;
    using Catel.Services;

    /// <summary>
    /// Manager that set command bindings of view`s view model.
    /// </summary>
    interface IViewRoutedCommandManager
    {
        /// <summary>
        /// Registers a view so it can be linked to a System.Windows.Input.CommandManager.
        /// </summary>
        /// <param name="view">The view to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        void RegisterView(IView view);

        /// <summary>
        /// Unregisters a view so it can no longer be linked to a System.Windows.Input.CommandManager.
        /// </summary>
        /// <param name="view">The view to unregister.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        void UnregisterView(IView view);
    }
}
