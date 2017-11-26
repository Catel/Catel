// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProgressStatus.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    /// <summary>
    /// The progress status interface.
    /// </summary>
    public interface IProgressStatus
    {
        /// <summary>
        /// Gets the current item.
        /// </summary>
        int CurrentItem { get; }

        /// <summary>
        /// Gets total items.
        /// </summary>
        int TotalItems { get; }

        /// <summary>
        /// Gets status format items.
        /// </summary>
        string StatusFormat { get; }
    }
}