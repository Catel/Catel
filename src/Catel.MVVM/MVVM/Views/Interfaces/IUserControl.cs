// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserControl.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using MVVM;

#if XAMARIN || XAMARIN_FORMS
    using ViewType = System.Object;
#elif UWP
    using ViewType = global::Windows.UI.Xaml.DependencyObject;
#else
    using ViewType = System.Windows.DependencyObject;

    using Windows.Controls;
#endif

    /// <summary>
    /// Interface defining functionality for user controls.
    /// </summary>
    public interface IUserControl : IView
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user control should close any existing
        /// view model when the control is unloaded from the visual tree.
        /// <para />
        /// Set this property to <c>false</c> if a view model should be kept alive and re-used
        /// for unloading/loading instead of creating a new one.
        /// <para />
        /// By default, this value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the view model should be closed when the control is unloaded; otherwise, <c>false</c>.
        /// </value>
        bool CloseViewModelOnUnloaded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether parent view model containers are supported. If supported,
        /// the user control will search for a <c>DependencyObject</c> that implements the <see cref="IViewModelContainer"/>
        /// interface. During this search, the user control will use both the visual and logical tree.
        /// <para />
        /// If a user control does not have any parent control implementing the <see cref="IViewModelContainer"/> interface, searching
        /// for it is useless and requires the control to search all the way to the top for the implementation. To prevent this from
        /// happening, set this property to <c>false</c>.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if parent view model containers are supported; otherwise, <c>false</c>.
        /// </value>
        bool SupportParentViewModelContainers { get; set; }

#if NET || NETCORE
        /// <summary>
        /// Gets or sets a value indicating whether to skip the search for an info bar message control. If not skipped,
        /// the user control will search for a the first <see cref="InfoBarMessageControl"/> that can be found. 
        /// During this search, the user control will use both the visual and logical tree.
        /// <para />
        /// If a user control does not have any <see cref="InfoBarMessageControl"/>, searching
        /// for it is useless and requires the control to search all the way to the top for the implementation. To prevent this from
        /// happening, set this property to <c>true</c>.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the search for an info bar message control should be skipped; otherwise, <c>false</c>.
        /// </value>
        bool SkipSearchingForInfoBarMessageControl { get; set; }
#endif

        /// <summary>
        /// Gets or sets a value indicating whether the user control should automatically be disabled when there is no
        /// active view model.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the user control should automatically be disabled when there is no active view model; otherwise, <c>false</c>.
        /// </value>
        bool DisableWhenNoViewModel { get; set; }

        /// <summary>
        /// Gets the parent of the view.
        /// </summary>
        /// <value>The parent.</value>
        ViewType Parent { get; }
    }
}
