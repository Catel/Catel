// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyRangedListChangedAction.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Collections
{
    /// <summary>
    /// Describes the real action performed on the <see cref="FastBindingList{T}"/>. 
    /// </summary>
    public enum NotifyRangedListChangedAction
    {
        /// <summary>
        /// Items was added to the <see cref="FastBindingList{T}"/>.
        /// </summary>
        Add,

        /// <summary>
        /// Items was removed from the <see cref="FastBindingList{T}"/>.
        /// </summary>
        Remove,

        /// <summary>
        /// The <see cref="FastBindingList{T}"/> has been reset.
        /// </summary>
        Reset
    }
}

#endif
