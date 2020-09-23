// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UICompletedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;

    /// <summary>
    /// This is the EventArgs return value for the IUIVisualizer.Show completed event.
    /// </summary>
    public class UICompletedEventArgs : EventArgs
    {
        public UICompletedEventArgs(UIVisualizerResult result)
        {
            Argument.IsNotNull("result", result);

            Result = result;
        }

        public UIVisualizerResult Result { get; }

        /// <summary>
        /// Gets the data context.
        /// </summary>
        /// <value>The data context.</value>
        public object DataContext { get { return Result.DataContext; } }

        /// <summary>
        /// Gets the result of the window.
        /// </summary>
        /// <value>The result.</value>
        public bool? DialogResult { get { return Result.DialogResult; } }
    }
}
