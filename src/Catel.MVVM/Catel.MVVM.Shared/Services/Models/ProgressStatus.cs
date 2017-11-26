// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressStatus.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    /// <summary>
    /// Class that represents a progress status.
    /// </summary>
    public class ProgressStatus : IProgressStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressStatus" /> class.
        /// </summary>
        /// <param name="currentItem">The curent item</param>
        /// <param name="totalItems">The total of items</param>
        /// <param name="statusFormat">The status format </param>
        public ProgressStatus(int currentItem, int totalItems, string statusFormat = "")
        {
            CurrentItem = currentItem;
            TotalItems = totalItems;
            StatusFormat = statusFormat;
        }

        /// <summary>
        /// Gets the current item.
        /// </summary>
        public int CurrentItem { get; }

        /// <summary>
        /// Gets total items.
        /// </summary>
        public int TotalItems { get; }

        /// <summary>
        /// Gets status format items.
        /// </summary>
        public string StatusFormat { get; }
    }
}