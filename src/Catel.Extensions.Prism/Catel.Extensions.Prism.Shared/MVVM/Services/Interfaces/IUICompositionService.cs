// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUICompositionService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;

    using Catel.MVVM;

#if PRISM6
    using Prism.Regions;
#else
    using Microsoft.Practices.Prism.Regions;
#endif


    /// <summary>
    /// The user interface composition service interface.
    /// </summary>
    public interface IUICompositionService
    {
        /// <summary>
        /// Activates a view into a specific <see cref="IRegion" /> via <see cref="IRegionManager" /> from a given view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="parentViewModel">The parent view model.</param>
        /// <param name="regionName">The region name.</param>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="System.InvalidOperationException">The <paramref name="viewModel" /> and <paramref name="parentViewModel" /> are equals.</exception>
        /// <exception cref="NotSupportedException">If the implementation of IRegionManager is not registered in the IoC container</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="parentViewModel" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="regionName" /> is <c>null</c> or whitespace.</exception>
        void Activate(IViewModel viewModel, IViewModel parentViewModel, string regionName);

        /// <summary>
        /// Activates a view into a specific <see cref="IRegion" /> via <see cref="IRegionManager" /> from a given view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="regionName">The region name.</param>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="NotSupportedException">If the implementation of IRegionManager is not registered in the IoC container</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        void Activate(IViewModel viewModel, string regionName = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        void Deactivate(IViewModel viewModel);

        /// <summary>
        /// Tries to activate an existing view model in the specified region name. If there is no view model alive, it will create one
        /// and navigate to that view model.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <param name="regionName">Name of the region.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModelType" /> is <c>null</c>.</exception>
        void Activate(Type viewModelType, string regionName);
    }
}