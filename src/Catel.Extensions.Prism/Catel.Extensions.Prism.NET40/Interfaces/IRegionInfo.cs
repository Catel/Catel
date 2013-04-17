// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRegionInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System.Windows;

    using Microsoft.Practices.Prism.Regions;

    /// <summary>
    /// The region info interface.
    /// </summary>
    public interface IRegionInfo
    {
        #region Properties

        /// <summary>
        /// Gets RegionName.
        /// </summary>
        string RegionName { get; }

        /// <summary>
        /// Gets RegionManager.
        /// </summary>
        IRegionManager RegionManager { get; }
        #endregion
    }
}