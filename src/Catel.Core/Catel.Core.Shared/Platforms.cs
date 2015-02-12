// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportedPlatforms.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;

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

        /// <summary>
        /// Determines whether the specified known platforms is currently supported.
        /// </summary>
        /// <param name="platformToCheck">The platform to check.</param>
        /// <returns><c>true</c> if the platform is supported; otherwise, <c>false</c>.</returns>
        public static bool IsPlatformSupported(KnownPlatforms platformToCheck)
        {
            return IsPlatformSupported(platformToCheck, CurrentPlatform);
        }

        /// <summary>
        /// Determines whether the specified known platforms is currently supported.
        /// </summary>
        /// <param name="platformToCheck">The platform to check.</param>
        /// <param name="currentPlatform">The current platform.</param>
        /// <returns><c>true</c> if the platform is supported; otherwise, <c>false</c>.</returns>
        public static bool IsPlatformSupported(KnownPlatforms platformToCheck, SupportedPlatforms currentPlatform)
        {
            switch (platformToCheck)
            {
                case KnownPlatforms.Unknown:
                    return false;

                case KnownPlatforms.NET:
                    return currentPlatform == SupportedPlatforms.NET40 ||
                           currentPlatform == SupportedPlatforms.NET45 ||
                           currentPlatform == SupportedPlatforms.NET50;

                case KnownPlatforms.NET40:
                    return currentPlatform == SupportedPlatforms.NET40;

                case KnownPlatforms.NET45:
                    return currentPlatform == SupportedPlatforms.NET45;

                case KnownPlatforms.NET50:
                    return currentPlatform == SupportedPlatforms.NET50;

                case KnownPlatforms.Silverlight:
                    return currentPlatform == SupportedPlatforms.Silverlight5;

                case KnownPlatforms.Silverlight5:
                    return currentPlatform == SupportedPlatforms.Silverlight5;

                case KnownPlatforms.WindowsPhone:
                    return currentPlatform == SupportedPlatforms.WindowsPhone80 ||
                           currentPlatform == SupportedPlatforms.WindowsPhone81Silverlight ||
                           currentPlatform == SupportedPlatforms.WindowsPhone81Runtime;

                case KnownPlatforms.WindowsPhoneSilverlight:
                    return currentPlatform == SupportedPlatforms.WindowsPhone80 ||
                           currentPlatform == SupportedPlatforms.WindowsPhone81Silverlight;

                case KnownPlatforms.WindowsPhoneRuntime:
                    return currentPlatform == SupportedPlatforms.WindowsPhone81Runtime;

                case KnownPlatforms.WindowsPhone80:
                    return currentPlatform == SupportedPlatforms.WindowsPhone80;

                case KnownPlatforms.WindowsPhone81Silverlight:
                    return currentPlatform == SupportedPlatforms.WindowsPhone81Silverlight;

                case KnownPlatforms.WindowsPhone81Runtime:
                    return currentPlatform == SupportedPlatforms.WindowsPhone81Runtime;

                case KnownPlatforms.WindowsRuntime:
                    return currentPlatform == SupportedPlatforms.WindowsRuntime80 ||
                           currentPlatform == SupportedPlatforms.WindowsRuntime81;

                case KnownPlatforms.WindowsRuntime80:
                    return currentPlatform == SupportedPlatforms.WindowsRuntime80;

                case KnownPlatforms.WindowsRuntime81:
                    return currentPlatform == SupportedPlatforms.WindowsRuntime81;

                case KnownPlatforms.Xamarin:
                    return currentPlatform == SupportedPlatforms.Android ||
                           currentPlatform == SupportedPlatforms.iOS;

                case KnownPlatforms.XamarinAndroid:
                    return currentPlatform == SupportedPlatforms.Android;

                case KnownPlatforms.XamariniOS:
                    return currentPlatform == SupportedPlatforms.iOS;

                case KnownPlatforms.PCL:
                    return currentPlatform == SupportedPlatforms.PCL;

                default:
                    throw new ArgumentOutOfRangeException("platformToCheck");
            }
        }

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
#elif SL5
            return SupportedPlatforms.Silverlight5;
#elif WP80
            return SupportedPlatforms.WindowsPhone80;
#elif WP81 && SILVERLIGHT
            return SupportedPlatforms.WindowsPhone81Silverlight;
#elif WP81 && NETFX_CORE
            return SupportedPlatforms.WindowsPhone81Runtime;
#elif WIN80
            return SupportedPlatforms.WindowsRuntime80;
#elif WIN81
            return SupportedPlatforms.WindowsRuntime81;
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
        /// Windows Phone 8.1 (Silverlight).
        /// </summary>
        WindowsPhone81Silverlight,

        /// <summary>
        /// Windows Phone 8.1 (Runtime).
        /// </summary>
        WindowsPhone81Runtime,

        /// <summary>
        /// Windows Runtime 8.0.
        /// </summary>
        WindowsRuntime80,

        /// <summary>
        /// Windows Runtime 8.1.
        /// </summary>
        WindowsRuntime81,

        /// <summary>
        /// The Android platform.
        /// </summary>
        Android,

        /// <summary>
        /// The iOS platform.
        /// </summary>
        iOS,

        /// <summary>
        /// Portable Class Library platform.
        /// </summary>
        PCL
    }

    /// <summary>
    /// Known platform groups.
    /// </summary>
    public enum KnownPlatforms
    {
        /// <summary>
        /// The current platform is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Any .NET platform.
        /// </summary>
        NET,

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
        /// Any sSilverlight platform.
        /// </summary>
        Silverlight,

        /// <summary>
        /// Silverlight 5.
        /// </summary>
        Silverlight5,

        /// <summary>
        /// Any Windows Phone platform.
        /// </summary>
        WindowsPhone,

        /// <summary>
        /// Any Windows Phone Silverlight platform.
        /// </summary>
        WindowsPhoneSilverlight,

        /// <summary>
        /// Any Windows Phone Runtime platform.
        /// </summary>
        WindowsPhoneRuntime,

        /// <summary>
        /// Windows Phone 8.0.
        /// </summary>
        WindowsPhone80,

        /// <summary>
        /// Windows Phone 8.1 (Silverlight).
        /// </summary>
        WindowsPhone81Silverlight,

        /// <summary>
        /// Windows Phone 8.1 (Runtime).
        /// </summary>
        WindowsPhone81Runtime,

        /// <summary>
        /// Any Windows Runtime platform.
        /// </summary>
        WindowsRuntime,

        /// <summary>
        /// Windows Runtime 8.0.
        /// </summary>
        WindowsRuntime80,

        /// <summary>
        /// Windows Runtime 8.1.
        /// </summary>
        WindowsRuntime81,

        /// <summary>
        /// Any Xamarin platform.
        /// </summary>
        Xamarin,

        /// <summary>
        /// The Xamarin Android platform.
        /// </summary>
        XamarinAndroid,

        /// <summary>
        /// The Xamarin iOS platform.
        /// </summary>
        XamariniOS,

        /// <summary>
        /// Portable Class Library platform.
        /// </summary>
        PCL
    }
}