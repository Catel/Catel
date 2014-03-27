// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelWrapperService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using Catel.Logging;
    using Catel.MVVM.Views;

    /// <summary>
    /// The view model wrapper service which is responsible of ensuring the view model container layer.
    /// </summary>
    public partial class ViewModelWrapperService : ViewModelWrapperServiceBase, IViewModelWrapperService
    {
        private const string InnerWrapperName = "__catelInnerWrapper";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Determines whether the specified view is already wrapped.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns><c>true</c> if the specified view is already wrapped; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        public bool IsWrapped(IView view)
        {
            Argument.IsNotNull("view", view);

            return IsViewWrapped(view);
        }

        /// <summary>
        /// Wraps the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModelSource">The view model source containing the <c>ViewModel</c> property.</param>
        /// <param name="wrapOptions">The wrap options.</param>
        /// <returns>The <see cref="IViewModelWrapper" />.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view" /> is <c>null</c>.</exception>
        public IViewModelWrapper Wrap(IView view, object viewModelSource, WrapOptions wrapOptions)
        {
            Argument.IsNotNull("view", view);
            Argument.IsNotNull("viewModelSource", viewModelSource);

#if XAMARIN
            throw new MustBeImplementedException();
#else
            return CreateViewModelGrid(view, viewModelSource, wrapOptions);
#endif
        }

        private object GetContent(IView view)
        {
            return view.Content;
        }

        private void SetContent(IView view, object content)
        {
            view.Content = content;
        }
    }
}