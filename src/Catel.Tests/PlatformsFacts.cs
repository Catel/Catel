// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlatformsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests
{
    using NUnit.Framework;

    public class PlatformsFacts
    {
        #region Nested type: TheIsPlatformSupportedMethod
        [TestFixture]
        public class TheIsPlatformSupportedMethod
        {
            [TestCase(KnownPlatforms.NET, SupportedPlatforms.NET45)]
            [TestCase(KnownPlatforms.NET, SupportedPlatforms.NET46)]
            [TestCase(KnownPlatforms.NET, SupportedPlatforms.NET47)]
            [TestCase(KnownPlatforms.NET, SupportedPlatforms.NET50)]
            [TestCase(KnownPlatforms.NET45, SupportedPlatforms.NET45)]
            [TestCase(KnownPlatforms.NET46, SupportedPlatforms.NET46)]
            [TestCase(KnownPlatforms.NET47, SupportedPlatforms.NET47)]
            [TestCase(KnownPlatforms.NET50, SupportedPlatforms.NET50)]
            [TestCase(KnownPlatforms.Xamarin, SupportedPlatforms.Android)]
            [TestCase(KnownPlatforms.Xamarin, SupportedPlatforms.iOS)]
            [TestCase(KnownPlatforms.XamarinAndroid, SupportedPlatforms.Android)]
            [TestCase(KnownPlatforms.XamariniOS, SupportedPlatforms.iOS)]
            public void ReturnsTrueForSupportedPlatform(KnownPlatforms platformToCheck, SupportedPlatforms currentPlatform)
            {
                Assert.IsTrue(Platforms.IsPlatformSupported(platformToCheck, currentPlatform));
            }

            [TestCase(KnownPlatforms.WindowsUniversal, SupportedPlatforms.NET47)]
            public void ReturnsFalseForUnsupportedPlatform(KnownPlatforms platformToCheck, SupportedPlatforms currentPlatform)
            {
                Assert.IsFalse(Platforms.IsPlatformSupported(platformToCheck, currentPlatform));
            }
        }
        #endregion
    }
}