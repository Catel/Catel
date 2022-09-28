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
                    return currentPlatform == SupportedPlatforms.NET6 ||
                           currentPlatform == SupportedPlatforms.NET7 ||
                           currentPlatform == SupportedPlatforms.NET8;

                case KnownPlatforms.NET6:
                    return currentPlatform == SupportedPlatforms.NET6;

                case KnownPlatforms.NET7:
                    return currentPlatform == SupportedPlatforms.NET7;

                case KnownPlatforms.NET8:
                    return currentPlatform == SupportedPlatforms.NET8;

                default:
                    throw new ArgumentOutOfRangeException("platformToCheck");
            }
        }

        private static SupportedPlatforms DeterminePlatform()
        {
#if NET6
            return SupportedPlatforms.NET6;
#elif NET7
            return SupportedPlatforms.NET7;
#elif NET8
            return SupportedPlatforms.NET8;
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
        /// .NET 6
        /// </summary>
        NET6,

        /// <summary>
        /// .NET 7
        /// </summary>
        NET7,

        /// <summary>
        /// .NET 8
        /// </summary>
        NET8
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
        /// .NET 6.
        /// </summary>
        NET6,

        /// <summary>
        /// .NET 7.
        /// </summary>
        NET7,

        /// <summary>
        /// .NET 8.
        /// </summary>
        NET8,
    }
}
