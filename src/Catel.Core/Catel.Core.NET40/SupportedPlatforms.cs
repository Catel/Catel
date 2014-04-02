// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportedPlatforms.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    /// <summary>
    /// Information about the platforms.
    /// </summary>
    public static class Platforms
    {
        /// <summary>
        /// Initializes static members of the <see cref="Platforms"/> class.
        /// </summary>
        static Platforms()
        {
            CurrentPlatform = DeterminePlatform();
        }

        /// <summary>
        /// Gets the current platform.
        /// </summary>
        /// <value>The current platform.</value>
        public static SupportedPlatforms CurrentPlatform { get; private set; }

        private static SupportedPlatforms DeterminePlatform()
        {
#if PCL
            return SupportedPlatforms.PCL;
#elif NET35
            throw new System.NotSupportedException("NET35 is not supported");
#elif NET40
            return SupportedPlatforms.NET40;
#elif NET45
            return SupportedPlatforms.NET45;
#elif NET50
            return SupportedPlatforms.NET50;
#elif SL4
            throw new System.NotSupportedException("SL4 is not supported");
#elif SL5
            return SupportedPlatforms.Silverlight5;
#elif WP7
            throw new System.NotSupportedException("WP7 is not supported");
#elif WP80
            return SupportedPlatforms.WindowsPhone80;
#elif WIN80
            return SupportedPlatforms.Windows80;
#elif WIN81
            return SupportedPlatforms.Windows81;
#elif ANDROID
            return SupportedPlatforms.Android;
#elif IOS
            return SupportedPlatforms.iOS;
#else
            throw new System.NotSupportedException("Unknown platform is not supported");
#endif
        }
    }

    /// <summary>
    /// All the available supported platforms.
    /// </summary>
    public enum SupportedPlatforms
    {
        /// <summary>
        /// .NET framework 4.0.
        /// </summary>
        NET40,

        /// <summary>
        /// .NET framework 4.5.
        /// </summary>
        NET45,

        /// <summary>
        /// .NET framework 5.0.
        /// </summary>
        NET50,

        /// <summary>
        /// Silverlight 5.
        /// </summary>
        Silverlight5,

        /// <summary>
        /// Windows Phone 8.0.
        /// </summary>
        WindowsPhone80,

        /// <summary>
        /// Windows Runtime 8.0.
        /// </summary>
        Windows80,

        /// <summary>
        /// Windows Runtime 8.1.
        /// </summary>
        Windows81,

        /// <summary>
        /// The portable class libraries (PCL).
        /// </summary>
        PCL,

        /// <summary>
        /// The Android platform.
        /// </summary>
        Android,

        /// <summary>
        /// The iOS platform.
        /// </summary>
        iOS
    }
}