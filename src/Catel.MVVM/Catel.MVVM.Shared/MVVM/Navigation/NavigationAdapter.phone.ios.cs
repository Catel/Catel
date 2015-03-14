// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.phone.ios.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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

        /// <summary>
        /// Gets the navigation URI for the target page.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        protected override string GetNavigationUri(object target)
        {
            throw new MustBeImplementedException();

            //var activity = target as Activity;
            //if (activity == null)
            //{
            //    return null;
            //}

            //return activity.LocalClassName;
        }
    }
}
#endif