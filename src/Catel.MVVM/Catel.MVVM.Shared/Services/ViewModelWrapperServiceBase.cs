// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelWrapperServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using Catel.MVVM.Views;

    /// <summary>
    /// Abstract class to enable partial abstract methods.
    /// </summary>
    public abstract class ViewModelWrapperServiceBase
    {
        /// <summary>
        /// Determines whether the specified view is wrapped.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns><c>true</c> if the view is wrapped; otherwise, <c>false</c>.</returns>
        protected abstract bool IsViewWrapped(IView view);
    }
}