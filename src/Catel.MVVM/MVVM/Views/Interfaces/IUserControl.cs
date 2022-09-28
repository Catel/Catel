namespace Catel.MVVM.Views
{
    using MVVM;
    using ViewType = System.Windows.DependencyObject;

    using Windows.Controls;

    /// <summary>
    /// Interface defining functionality for user controls.
    /// </summary>
    public interface IUserControl : IView
    {
        /// <summary>
        /// Gets or sets a the view model lifetime management.
        /// <para />
        /// By default, this value is <see cref="ViewModelLifetimeManagement"/>.
        /// </summary>
        /// <value>
        /// The view model lifetime management.
        /// </value>
        ViewModelLifetimeManagement ViewModelLifetimeManagement { get; set; }

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
