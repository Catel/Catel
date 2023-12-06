namespace Catel.Tests
{
    using NUnit.Framework;

    public class PlatformsFacts
    {
        #region Nested type: TheIsPlatformSupportedMethod
        [TestFixture, Explicit]
        public class TheIsPlatformSupportedMethod
        {
            [TestCase(KnownPlatforms.NET, SupportedPlatforms.NET6)]
            [TestCase(KnownPlatforms.NET, SupportedPlatforms.NET7)]
            [TestCase(KnownPlatforms.NET, SupportedPlatforms.NET8)]
            public void ReturnsTrueForSupportedPlatform(KnownPlatforms platformToCheck, SupportedPlatforms currentPlatform)
            {
                Assert.That(Platforms.IsPlatformSupported(platformToCheck, currentPlatform), Is.True);
            }

            //[TestCase(KnownPlatforms.N, SupportedPlatforms.NET47)]
            public void ReturnsFalseForUnsupportedPlatform(KnownPlatforms platformToCheck, SupportedPlatforms currentPlatform)
            {
                Assert.That(Platforms.IsPlatformSupported(platformToCheck, currentPlatform), Is.False);
            }
        }
        #endregion
    }
}
