// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavingEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    /// <summary>
    /// EventArgs for the <see cref="IViewModel.SavingAsync"/> event.
    /// </summary>
    public class SavingEventArgs : CancellableEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SavingEventArgs"/> class.
        /// </summary>
        public SavingEventArgs()
        {
        }
    }
}