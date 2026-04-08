namespace Catel.Tests.ComponentModel;

using Catel.Services;
using Moq;
using NUnit.Framework;

public class DisplayNameAttributeFacts
{
    [TestFixture]
    public class The_DisplayName_Property
    {
        [TestCase]
        public void Returns_Translated_Resource_Name()
        {
            var languageServiceMock = new Mock<ILanguageService>();
            languageServiceMock.Setup(x => x.GetString("MyDisplayName"))
                .Returns<string>(x => "It works");

            var displayAttribute = new Catel.ComponentModel.DisplayNameAttribute(languageServiceMock.Object, "MyDisplayName");

            Assert.That(displayAttribute.DisplayName, Is.EqualTo("It works"));
        }

        [TestCase]
        public void Returns_Resource_Name_If_Translation_Cannot_Be_Found()
        {
            var languageServiceMock = new Mock<ILanguageService>();
            languageServiceMock.Setup(x => x.GetString("MyDisplayName"))
                .Returns<string>(x => "It works");

            var displayAttribute = new Catel.ComponentModel.DisplayNameAttribute(languageServiceMock.Object, "MyNonExistingDisplayName");

            Assert.That(displayAttribute.DisplayName, Is.EqualTo("MyNonExistingDisplayName"));
        }
    }
}
