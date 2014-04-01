// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.phone.ios.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if IOS
namespace Catel.MVVM.Navigation
{
    public partial class NavigationAdapter
    {
        /// <summary>
        /// Determines whether the navigation can be handled by this adapter.
        /// </summary>
        /// <returns><c>true</c> if the navigation can be handled by this adapter; otherwise, <c>false</c>.</returns>
        protected override bool CanHandleNavigation()
        {
            throw new MustBeImplementedException();
        }
    }
}
#endif