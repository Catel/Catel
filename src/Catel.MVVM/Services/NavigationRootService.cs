// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationRootService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    /// <summary>
    /// Service to retrieve the navigation root in the application.
    /// </summary>
    public partial class NavigationRootService : INavigationRootService
    {
#if XAMARIN_FORMS
        public object GetNavigationRoot()
        {
            throw new System.NotImplementedException();
        }
#endif
    }
}

