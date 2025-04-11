namespace Catel.Tests.Services
{
    using System;
    using System.Globalization;
    using Catel.Services;
    using Moq;
    using NUnit.Framework;

    public partial class ILanguageServiceExtensionsFacts
    {
        [TestFixture]
        public class The_GetRequiredStringAndFormat_Method
        {
            [Test]
            public void ThrowsArgumentNullException_WhenLanguageServiceIsNull()
            {
                Assert.Throws<ArgumentNullException>(() =>
                {
                    ILanguageService languageService = null;
                    languageService.GetRequiredStringAndFormat("ResourceName");
                });
            }

            [Test]
            public void ThrowsCatelException_WhenResourceNotFound()
            {
                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock
                    .Setup(ls => ls.GetString(It.IsAny<string>()))
                    .Returns((string)null);

                Assert.Throws<CatelException>(() =>
                {
                    languageServiceMock.Object.GetRequiredStringAndFormat("NonExistentResource");
                });
            }

            [Test]
            public void ReturnsString_WhenResourceIsFound()
            {
                const string expectedString = "Hello, World!";

                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock
                    .Setup(ls => ls.GetString("Greeting"))
                    .Returns(expectedString);

                var result = languageServiceMock.Object.GetRequiredStringAndFormat("Greeting");

                Assert.That(result, Is.EqualTo(expectedString));
            }

            [Test]
            public void ReturnsString_WhenResourceIsFound_WithArguments()
            {
                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock
                    .Setup(ls => ls.GetString("Greeting"))
                    .Returns("Hello, {0}!");

                var result = languageServiceMock.Object.GetRequiredStringAndFormat("Greeting", "World");

                Assert.That(result, Is.EqualTo("Hello, World!"));
            }

            [Test]
            public void ReturnsString_WithCultureInfo_WhenResourceIsFound()
            {
                const string expectedString = "Bonjour, le monde!";
                var cultureInfo = new CultureInfo("fr-FR");

                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock
                    .Setup(ls => ls.GetString("Greeting", cultureInfo))
                    .Returns(expectedString);

                var result = languageServiceMock.Object.GetRequiredStringAndFormat("Greeting", cultureInfo);

                Assert.That(result, Is.EqualTo(expectedString));
            }

            [Test]
            public void ReturnsString_WithCultureInfo_WhenResourceIsFound_WithArguments()
            {
                var cultureInfo = new CultureInfo("fr-FR");

                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock
                    .Setup(ls => ls.GetString("Greeting", cultureInfo))
                    .Returns("Bonjour, {0}!");

                var result = languageServiceMock.Object.GetRequiredStringAndFormat("Greeting", cultureInfo, "le monde");

                Assert.That(result, Is.EqualTo("Bonjour, le monde!"));
            }

            [Test]
            public void ThrowsCatelException_WithCultureInfo_WhenResourceNotFound()
            {
                var cultureInfo = new CultureInfo("fr-FR");

                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock
                    .Setup(ls => ls.GetString(It.IsAny<string>(), cultureInfo))
                    .Returns((string)null);

                Assert.Throws<CatelException>(() =>
                {
                    languageServiceMock.Object.GetRequiredStringAndFormat("NonExistentResource", cultureInfo);
                });
            }

            [Test]
            public void ReturnsString_WithLanguageSourceAndCultureInfo_WhenResourceIsFound()
            {
                const string expectedString = "Hola, Mundo!";
                var cultureInfo = new CultureInfo("es-ES");
                var languageSourceMock = new Mock<ILanguageSource>();

                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock
                    .Setup(ls => ls.GetString(languageSourceMock.Object, "Greeting", cultureInfo))
                    .Returns(expectedString);

                var result = languageServiceMock.Object.GetRequiredStringAndFormat(languageSourceMock.Object, "Greeting", cultureInfo);

                Assert.That(result, Is.EqualTo(expectedString));
            }

            [Test]
            public void ReturnsString_WithLanguageSourceAndCultureInfo_WhenResourceIsFound_WithArguments()
            {
                var cultureInfo = new CultureInfo("es-ES");
                var languageSourceMock = new Mock<ILanguageSource>();

                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock
                    .Setup(ls => ls.GetString(languageSourceMock.Object, "Greeting", cultureInfo))
                    .Returns("Hola, {0}!");

                var result = languageServiceMock.Object.GetRequiredStringAndFormat(languageSourceMock.Object, "Greeting", cultureInfo, "Mundo");

                Assert.That(result, Is.EqualTo("Hola, Mundo!"));
            }

            [Test]
            public void ThrowsCatelException_WithLanguageSourceAndCultureInfo_WhenResourceNotFound()
            {
                var cultureInfo = new CultureInfo("es-ES");
                var languageSourceMock = new Mock<ILanguageSource>();

                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock
                    .Setup(ls => ls.GetString(languageSourceMock.Object, It.IsAny<string>(), cultureInfo))
                    .Returns((string)null);

                Assert.Throws<CatelException>(() =>
                {
                    languageServiceMock.Object.GetRequiredStringAndFormat(languageSourceMock.Object, "NonExistentResource", cultureInfo);
                });
            }
        }
    }
}
