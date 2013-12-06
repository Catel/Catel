// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotSupportedInWindows8Exception.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;

    /// <summary>
    /// Exception in case the functionality is not supported in Windows 8.
    /// <para />
    /// Unfortunately, Windows 8 misses a lot of functionality. When a feature is not supported in Catel, 
    /// this is because the .NET Framework (or actually Windows 8) does not allow the code to handle 
    /// that specific feature.
    /// </summary>
    [ObsoleteEx(Message = "No longer supported", Replacement = "NotSupportedInPlatformException", TreatAsErrorFromVersion = "3.8", RemoveInVersion = "4.0")]
    public class NotSupportedInWindows8Exception : Exception
    {
        #region Constructor & destructor
        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedInWindows8Exception"/> class.
        /// </summary>
        /// <param name="reasonFormat">The reason format.</param>
        /// <param name="args">The formatting arguments.</param>
        public NotSupportedInWindows8Exception(string reasonFormat = "", params object[] args)
            : base(ResourceHelper.GetString(typeof(NotSupportedInWindows8Exception), "Exceptions", "NotSupportedInWindows8"))
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