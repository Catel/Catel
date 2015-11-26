// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRegionManagerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.Windows.Controls;
#if PRISM6
    using Prism.Regions;
#else
    using Microsoft.Practices.Prism.Regions;
#endif

    /// <summary>
    /// The <see cref="IRegionManager"/> extensions methods.
    /// </summary>
    public static class IRegionManagerExtensions
    {
        #region Methods
        /// <summary>
        /// Registers the view with region.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="regionName">Name of the region.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="regionManager"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="regionName"/> is <c>null</c>.</exception>
        public static void RegisterViewWithRegion<TView>(this IRegionManager regionManager, string regionName) 
            where TView : UserControl
        {
            Argument.IsNotNull("regionManager", regionManager);
            Argument.IsNotNullOrWhitespace("regionName", regionName);

            regionManager.RegisterViewWithRegion(regionName, typeof (TView));
        }
        #endregion
    }
}