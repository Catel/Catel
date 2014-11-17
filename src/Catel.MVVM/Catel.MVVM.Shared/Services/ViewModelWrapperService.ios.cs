// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelWrapperService.ios.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if IOS

namespace Catel.Services
{
    using Catel.MVVM.Views;

    public partial class ViewModelWrapperService
    {
        /// <summary>
        /// Determines whether the specified view is wrapped.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns><c>true</c> if the view is wrapped; otherwise, <c>false</c>.</returns>
        protected override bool IsViewWrapped(IView view)
        {
            return true;
        }
    }
}

#endif