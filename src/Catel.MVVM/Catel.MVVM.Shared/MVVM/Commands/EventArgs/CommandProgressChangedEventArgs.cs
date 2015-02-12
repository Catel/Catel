// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandProgressChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Event args for event when <see cref="TaskCommand"/> progress changes.
    /// </summary>
    public class CommandProgressChangedEventArgs<TProgress> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProgressChangedEventArgs{TProgress}"/> class.
        /// </summary>
        /// <param name="progress"></param>
        public CommandProgressChangedEventArgs(TProgress progress)
        {
            Progress = progress;
        }

        /// <summary>
        /// Progress info.
        /// </summary>
        public TProgress Progress { get; set; }
    }
}