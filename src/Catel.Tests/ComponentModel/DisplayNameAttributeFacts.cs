namespace Catel.Tests.ComponentModel
{
    using Catel.Services;
    using Moq;
    using NUnit.Framework;

    public class DisplayNameAttributeFacts
    {
        [TestFixture]
        public class TheDisplayNameProperty
        {
            [TestCase]
            public void ReturnsTranslatedResourceName()
            {
                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock.Setup(x => x.GetString(It.IsAny<string>()))
                    .Returns<string>(x => "It works");

                var displayAttribute = new Catel.ComponentModel.DisplayNameAttribute(languageServiceMock.Object, "MyDisplayName");
 
                Assert.That(displayAttribute.DisplayName, Is.EqualTo("It works"));
            }

            [TestCase]
            public void ReturnsResourceNameIfTranslationCannotBeFound()
            {
                var languageServiceMock = new Mock<ILanguageService>();
                languageServiceMock.Setup(x => x.GetString(It.IsAny<string>()))
                    .Returns<string>(x => "It works");

                var displayAttribute = new Catel.ComponentModel.DisplayNameAttribute(languageServiceMock.Object, "MyNonExistingDisplayName");

                Assert.That(displayAttribute.DisplayName, Is.EqualTo("MyNonExistingDisplayName"));
            }
        }
    }
}
