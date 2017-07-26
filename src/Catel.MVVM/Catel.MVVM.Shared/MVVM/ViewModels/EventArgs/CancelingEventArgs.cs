// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelingEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    /// <summary>
    /// EventArgs for the <see cref="IViewModel.CancelingAsync"/> event.
    /// </summary>
    public class CancelingEventArgs : CancellableEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelingEventArgs"/> class.
        /// </summary>
        public CancelingEventArgs()
        {
        }
    }
}