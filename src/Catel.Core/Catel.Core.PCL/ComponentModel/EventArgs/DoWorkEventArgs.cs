// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoWorkEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace System.ComponentModel
{
    /// <summary>
    /// Provides data for the <see cref="BackgroundWorker.DoWork"/> event.
    /// </summary>
    /// <remarks>
    /// This code originally comes from https://pclcontrib.codeplex.com/SourceControl/latest#Source/Portable.ComponentModel.Async/ComponentModel/DoWorkEventArgs.cs.
    /// </remarks>
    public class DoWorkEventArgs : CancelEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DoWorkEventArgs"/> class.
        /// </summary>
        /// <param name="argument">
        /// An <see cref="object"/> representing the argument of an asynchronous operation.
        /// </param>
        public DoWorkEventArgs(object argument)
        {
            Argument = argument;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value that represents the argument of an asynchronous operation.
        /// </summary>
        /// <value>An <see cref="object" /> representing the argument of an asynchronous operation.</value>
        public object Argument { get; private set; }

        /// <summary>
        /// Gets or sets a value that represents the result of an asynchronous operation.
        /// </summary>
        /// <value>An <see cref="object" /> representing the result of an asynchronous operation.</value>
        public object Result { get; set; }
        #endregion
    }
}