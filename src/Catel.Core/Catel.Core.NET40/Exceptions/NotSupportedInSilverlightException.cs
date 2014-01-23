// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotSupportedInSilverlightException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;

    /// <summary>
    /// Exception in case the functionality is not supported in Silverlight.
    /// <para />
    /// Unfortunately, Silverlight misses a lot of functionality. When a feature is not supported in Catel, 
    /// this is because the .NET Framework (or actually Silverlight) does not allow the code to handle 
    /// that specific feature.
    /// </summary>
    [ObsoleteEx(Message = "No longer supported", Replacement = "NotSupportedInPlatformException", TreatAsErrorFromVersion = "3.8", RemoveInVersion = "4.0")]
    public class NotSupportedInSilverlightException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedInSilverlightException"/> class.
        /// </summary>
        /// <param name="reasonFormat">The reason format.</param>
        /// <param name="args">The formatting arguments.</param>
        public NotSupportedInSilverlightException(string reasonFormat = "", params object[] args)
            : base(ResourceHelper.GetString("NotSupportedInSilverlight"))
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