// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewRegionItem.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System.Windows;

    using Microsoft.Practices.Prism.Regions;

    /// <summary>
    /// The view region item interface.
    /// </summary>
    public interface IViewInfo
    {
        /// <summary>
        /// Gets View.
        /// </summary>
        FrameworkElement View { get; }

        /// <summary>
        /// Gets Region.
        /// </summary>
        IRegion Region { get; }
    }
}