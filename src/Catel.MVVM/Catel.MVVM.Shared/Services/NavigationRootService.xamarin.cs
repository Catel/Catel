// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationRootService.xamarin.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if XAMARIN || XAMARIN_FORMS

namespace Catel.Services
{
    public partial class NavigationRootService
    {
        /// <summary>
        /// Gets the navigation root.
        /// </summary>
        /// <returns>System.Object.</returns>
        public virtual object GetNavigationRoot()
        {
            return null;
        }
    }
}

#endif