// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MustBeImplementedException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;

    /// <summary>
    /// Exception in case the functionality is not yet implemented but is supported in the current platform.
    /// <para />
    /// Unfortunately, the team has limited resources and must focus on the most requested features. Feel free to
    /// create a pull request or notify the team that you are missing this feature.
    /// </summary>
    public class MustBeImplementedException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedInPlatformException"/> class.
        /// </summary>
        public MustBeImplementedException()
            : base("Unfortunately, the team has limited resources and must focus on the most requested features. Feel free to create a pull request or notify the team that you are missing this feature.")
        {
        }
        #endregion
    }
}