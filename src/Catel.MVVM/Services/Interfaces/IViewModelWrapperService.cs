// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModelWrapperService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using Catel.MVVM.Views;

    /// <summary>
    /// Available wrap options.
    /// </summary>
    [Flags]
    public enum WrapOptions
    {
        /// <summary>
        /// No options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Create warning and error validator for view model.
        /// </summary>
        CreateWarningAndErrorValidatorForViewModel = 1,

        /// <summary>
        /// Event when the content is <c>null</c>, create a wrapper.
        /// </summary>
        Force = 2
    }

    /// <summary>
    /// The view model wrapper service which is responsible of ensuring the view model container layer.
    /// </summary>
    public interface IViewModelWrapperService
    {
        /// <summary>
        /// Determines whether the specified view is already wrapped.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns><c>true</c> if the specified view is already wrapped; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        bool IsWrapped(IView view);

        /// <summary>
        /// Wraps the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModelSource">The view model source containing the <c>ViewModel</c> property.</param>
        /// <param name="wrapOptions">The wrap options.</param>
        /// <returns>The <see cref="IViewModelWrapper" />.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view" /> is <c>null</c>.</exception>
        IViewModelWrapper Wrap(IView view, object viewModelSource, WrapOptions wrapOptions);

        /// <summary>
        /// Gets the existing view model wrapper for the view. If there is none, this method will return <c>null</c>.
        /// </summary>
        /// <param name="view">The view to get the wrapper for.</param>
        /// <returns>The existing view model wrapper or <c>null</c> if there is no wrapper.</returns>
        IViewModelWrapper GetWrapper(IView view);
    }
}
