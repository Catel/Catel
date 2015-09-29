// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUICompositionServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.Linq;
    using IoC;
    using MVVM;

    /// <summary>
    /// Extension methods for the UI composition service.
    /// </summary>
    public static class IUICompositionServiceExtensions
    {
        /// <summary>
        /// Tries to activate an existing view model in the specified region name. If there is no view model alive, it will create one
        /// and navigate to that view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="uiCompositionService">The UI composition service.</param>
        /// <param name="regionName">Name of the region.</param>
        public static void Activate<TViewModel>(this IUICompositionService uiCompositionService, string regionName)
            where TViewModel : IViewModel
        {
            uiCompositionService.Activate((IViewModel) typeof(TViewModel), regionName);
        }
    }
}