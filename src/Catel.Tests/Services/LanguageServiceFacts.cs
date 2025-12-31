namespace Catel.Tests.Services
{
    using System;
    using Catel.Services;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;

    public class LanguageServiceFacts
    {
        [TestFixture]
        public class TheRegisterLanguageSourceMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullLanguageSource()
            {
                var languageService = new LanguageService(new NullLogger<LanguageService>(),
                    new[]
                    {
                        new LanguageResourceSource("Catel.MVVM", "Catel.Properties", "Resources")
                    });

                Assert.Throws<ArgumentNullException>(() => languageService.RegisterLanguageSource(null));
            }
        }

        [TestFixture]
        public class TheGetStringMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullResourceName()
            {
                var languageService = new LanguageService(new NullLogger<LanguageService>(),
                    new[]
                    {
                        new LanguageResourceSource("Catel.MVVM", "Catel.Properties", "Resources")
                    });

                Assert.Throws<ArgumentException>(() => languageService.GetString(null));
            }

            [TestCase]
            public void ReturnsNullForNonExistingResource()
            {
                var languageService = new LanguageService(new NullLogger<LanguageService>(),
                    new[]
                    {
                        new LanguageResourceSource("Catel.MVVM", "Catel.Properties", "Resources")
                    });

                Assert.That(languageService.GetString("NonExistingResourceName"), Is.EqualTo(null));
            }

            //[TestCase]
            //public void ReturnsStringForCoreAssembly()
            //{
            //var languageService = new LanguageService(new NullLogger<LanguageService>(),
            //    new[]
            //    {
            //            new LanguageResourceSource("Catel.MVVM", "Catel.Properties", "Resources")
            //    });

            //    Assert.AreEqual("{0} has the following warnings:", languageService.GetString("WarningsFound"));
            //}

            [TestCase]
            public void ReturnsStringForMvvmAssembly()
            {
                var languageService = new LanguageService(new NullLogger<LanguageService>(),
                    new[]
                    {
                        new LanguageResourceSource("Catel.MVVM", "Catel.Properties", "Resources")
                    });

                Assert.That(languageService.GetString("WarningTitle"), Is.EqualTo("Warning"));
            }
        }
    }
}
