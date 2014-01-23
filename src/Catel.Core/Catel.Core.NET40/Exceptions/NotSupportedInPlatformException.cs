// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotSupportedInPlatformException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;

    /// <summary>
    /// Exception in case the functionality is not supported in the current platform.
    /// <para />
    /// Unfortunately, some platforms miss a lot of functionality. When a feature is not supported in Catel, 
    /// this is because the .NET Framework (or actually the specified platform) does not allow the code to handle 
    /// that specific feature.
    /// </summary>
    public class NotSupportedInPlatformException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedInPlatformException"/> class.
        /// </summary>
        /// <param name="featureFormat">The feature format.</param>
        /// <param name="args">The formatting arguments.</param>
        public NotSupportedInPlatformException(string featureFormat = "", params object[] args)
            : base(ResourceHelper.GetString("NotSupportedInPlatform"))
        {
            Reason = string.Format(featureFormat, args);
            Platform = Platforms.CurrentPlatform;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the reason why the feature is not supported.
        /// </summary>
        /// <value>The reason why the feature is missing.</value>
        public string Reason { get; private set; }

        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <value>The platform.</value>
        public SupportedPlatforms Platform { get; private set; }
        #endregion
    }
}