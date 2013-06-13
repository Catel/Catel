// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotSupportedInWinRTException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;

    /// <summary>
    /// Exception in case the functionality is not supported in Windows RT.
    /// <para />
    /// Unfortunately, Windows Phone 7 misses a lot of functionality. When a feature is not supported in Catel, 
    /// this is because the .NET Framework (or actually Windows Phone 7) does not allow the code to handle 
    /// that specific feature.
    /// </summary>
    public class NotSupportedInWinRTException : Exception
    {
        #region Constructor & destructor
        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedInWinRTException"/> class.
        /// </summary>
        /// <param name="reasonFormat">The reason format.</param>
        /// <param name="args">The formatting arguments.</param>
        public NotSupportedInWinRTException(string reasonFormat = "", params object[] args)
            : base(ResourceHelper.GetString(typeof(NotSupportedInWinRTException), "Exceptions", "NotSupportedInWinRT"))
        {
            Reason = string.Format(reasonFormat, args);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the reason why the feature is not supported.
        /// </summary>
        /// <value>The reason why the feature is missing.</value>
        public string Reason { get; private set; }
        #endregion
    }
}