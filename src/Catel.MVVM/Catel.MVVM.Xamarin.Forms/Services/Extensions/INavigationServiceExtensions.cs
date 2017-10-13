// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INavigationServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System.Linq;

    /// <summary>
    /// The INavigationService extensions methods
    /// </summary>
    public static class INavigationServiceExtensions
    {
        /// <summary>
        /// Navigates the specified location registered using the view model type.
        /// </summary>
        /// <typeparam name="TViewModelType">The view model type.</typeparam>
        /// <param name="this">The service instance</param>
        /// <param name="parameters">The parameters</param>
        public static void Navigate<TViewModelType>(this INavigationService @this, params object[] parameters)
        {
            if (parameters != null)
            {
                int i = 0;
                @this.Navigate<TViewModelType>(parameters.ToDictionary(o => "p" + i++));
            }
            else
            {
                @this.Navigate<TViewModelType>();
            }
        }
    }
}
