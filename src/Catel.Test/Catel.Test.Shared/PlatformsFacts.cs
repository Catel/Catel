// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlatformsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test
{
    using NUnit.Framework;

    public class PlatformsFacts
    {
        #region Nested type: TheIsPlatformSupportedMethod
        [TestFixture]
        public class TheIsPlatformSupportedMethod
        {
            [TestCase(KnownPlatforms.NET, SupportedPlatforms.NET40)]
            [TestCase(KnownPlatforms.NET, SupportedPlatforms.NET45)]
            [TestCase(KnownPlatforms.NET, SupportedPlatforms.NET50)]
            [TestCase(KnownPlatforms.NET40, SupportedPlatforms.NET40)]
            [TestCase(KnownPlatforms.NET45, SupportedPlatforms.NET45)]
            [TestCase(KnownPlatforms.NET50, SupportedPlatforms.NET50)]
            [TestCase(KnownPlatforms.Silverlight, SupportedPlatforms.Silverlight5)]
            [TestCase(KnownPlatforms.Silverlight5, SupportedPlatforms.Silverlight5)]
            [TestCase(KnownPlatforms.WindowsPhone, SupportedPlatforms.WindowsPhone80)]
            [TestCase(KnownPlatforms.WindowsPhone, SupportedPlatforms.WindowsPhone81Silverlight)]
            [TestCase(KnownPlatforms.WindowsPhone, SupportedPlatforms.WindowsPhone81Runtime)]
            [TestCase(KnownPlatforms.WindowsPhone80, SupportedPlatforms.WindowsPhone80)]
            [TestCase(KnownPlatforms.WindowsPhone81Silverlight, SupportedPlatforms.WindowsPhone81Silverlight)]
            [TestCase(KnownPlatforms.WindowsPhone81Runtime, SupportedPlatforms.WindowsPhone81Runtime)]
            [TestCase(KnownPlatforms.WindowsRuntime, SupportedPlatforms.WindowsRuntime80)]
            [TestCase(KnownPlatforms.WindowsRuntime, SupportedPlatforms.WindowsRuntime81)]
            [TestCase(KnownPlatforms.WindowsRuntime80, SupportedPlatforms.WindowsRuntime80)]
            [TestCase(KnownPlatforms.WindowsRuntime81, SupportedPlatforms.WindowsRuntime81)]
            [TestCase(KnownPlatforms.Xamarin, SupportedPlatforms.Android)]
            [TestCase(KnownPlatforms.Xamarin, SupportedPlatforms.iOS)]
            [TestCase(KnownPlatforms.XamarinAndroid, SupportedPlatforms.Android)]
            [TestCase(KnownPlatforms.XamariniOS, SupportedPlatforms.iOS)]
            [TestCase(KnownPlatforms.PCL, SupportedPlatforms.PCL)]
            public void ReturnsTrueForSupportedPlatform(KnownPlatforms platformToCheck, SupportedPlatforms currentPlatform)
            {
                Assert.IsTrue(Platforms.IsPlatformSupported(platformToCheck, currentPlatform));
            }

            [TestCase(KnownPlatforms.Silverlight, SupportedPlatforms.NET40)]
            public void ReturnsFalseForUnsupportedPlatform(KnownPlatforms platformToCheck, SupportedPlatforms currentPlatform)
            {
                Assert.IsFalse(Platforms.IsPlatformSupported(platformToCheck, currentPlatform));
            }
        }
        #endregion
    }
}