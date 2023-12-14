namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Interface that allows a view model to be used in a nested user controls scenario.
    /// </summary>
    public interface IRelationalViewModel : IViewModel
    {
        /// <summary>
        /// Gets the parent view model.
        /// </summary>
        /// <value>The parent view model.</value>
        IViewModel? ParentViewModel { get; }

        /// <summary>
        /// Sets the new parent view model of this view model.
        /// </summary>
        /// <param name="parentViewModel">The parent view model.</param>
        void SetParentViewModel(IViewModel? parentViewModel);

        /// <summary>
        /// Registers a child view model to this view model. When a view model is registered as a child view model, it will
        /// receive all notifications from this view model and be notified of any validation changes.
        /// </summary>
        /// <param name="childViewModel">The child view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="childViewModel"/> is <c>null</c>.</exception>
        void RegisterChildViewModel(IViewModel childViewModel);

        /// <summary>
        /// Unregisters the child view model. This means that the child view model will no longer receive any notifications
        /// from this view model as parent view model, nor will it be included in any validation calls in this view model.
        /// </summary>
        /// <param name="childViewModel">The child.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="childViewModel"/> is <c>null</c>.</exception>
        void UnregisterChildViewModel(IViewModel childViewModel);
    }
}
