// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportedPlatforms.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using Catel.Reflection;

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
                    return currentPlatform == SupportedPlatforms.NET45 ||
                           currentPlatform == SupportedPlatforms.NET46 ||
                           currentPlatform == SupportedPlatforms.NET47 ||
                           currentPlatform == SupportedPlatforms.NET50;

                case KnownPlatforms.NET45:
                    return currentPlatform == SupportedPlatforms.NET45;

                case KnownPlatforms.NET46:
                    return currentPlatform == SupportedPlatforms.NET46;

                case KnownPlatforms.NET47:
                    return currentPlatform == SupportedPlatforms.NET47;

                case KnownPlatforms.NET50:
                    return currentPlatform == SupportedPlatforms.NET50;

                case KnownPlatforms.NetStandard:
                    return currentPlatform == SupportedPlatforms.NetStandard20;

                case KnownPlatforms.NetStandard20:
                    return currentPlatform == SupportedPlatforms.NetStandard20;

                case KnownPlatforms.WindowsUniversal:
                    return currentPlatform == SupportedPlatforms.WindowsUniversal;

                case KnownPlatforms.Xamarin:
                    return currentPlatform == SupportedPlatforms.Android ||
                           currentPlatform == SupportedPlatforms.iOS;

                case KnownPlatforms.XamarinAndroid:
                    return currentPlatform == SupportedPlatforms.Android;

                case KnownPlatforms.XamariniOS:
                    return currentPlatform == SupportedPlatforms.iOS;

                case KnownPlatforms.XamarinForms:
                    return currentPlatform == SupportedPlatforms.NetStandard20;

                default:
                    throw new ArgumentOutOfRangeException("platformToCheck");
            }
        }

        private static SupportedPlatforms DeterminePlatform()
        {
            // Note: don't use cache service, it forces the initialization of the AppDomain which
            // might be too early
            if (Type.GetType("Xamarin.Forms.Device") is not null)
            {
                return SupportedPlatforms.XamarinForms;
            }

#if NET45
            return SupportedPlatforms.NET45;
#elif NET46
            return SupportedPlatforms.NET46;
#elif NET47
            return SupportedPlatforms.NET47;
#elif NET50
            return SupportedPlatforms.NET50;
#elif NS20
            return SupportedPlatforms.NetStandard20;
#elif UWP
            return SupportedPlatforms.WindowsUniversal;
#elif ANDROID
            return SupportedPlatforms.Android;
#elif IOS
            return SupportedPlatforms.iOS;
#elif XAMARIN_FORMS
            return SupportedPlatforms.XamarinForms;
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
        /// .NET framework 4.5.
        /// </summary>
        NET45,

        /// <summary>
        /// .NET framework 4.6.
        /// </summary>
        NET46,

        /// <summary>
        /// .NET framework 4.7.
        /// </summary>
        NET47,

        /// <summary>
        /// .NET framework 5.0.
        /// </summary>
        NET50,

        /// <summary>
        /// .NET Standard 2.0.
        /// </summary>
        NetStandard20,

        /// <summary>
        /// Windows Universal 10.0.
        /// </summary>
        WindowsUniversal,

        /// <summary>
        /// The Android platform.
        /// </summary>
        Android,

        /// <summary>
        /// The iOS platform.
        /// </summary>
        iOS,

        /// <summary>
        /// Xamarin.Forms platform.
        /// </summary>
        XamarinForms,
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
        /// .NET framework 4.5.
        /// </summary>
        NET45,

        /// <summary>
        /// .NET framework 4.6.
        /// </summary>
        NET46,

        /// <summary>
        /// .NET framework 4.7.
        /// </summary>
        NET47,

        /// <summary>
        /// .NET framework 5.0.
        /// </summary>
        NET50,

        /// <summary>
        /// Any .NET Standard platform.
        /// </summary>
        NetStandard,

        /// <summary>
        /// .NET Standard 2.0.
        /// </summary>
        NetStandard20,

        /// <summary>
        /// Any Windows Universal platform.
        /// </summary>
        WindowsUniversal,

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
        /// The xamarin forms platform.
        /// </summary>
        XamarinForms,
    }
}