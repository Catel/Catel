namespace Catel.Tests.ComponentModel
{
    using NUnit.Framework;
    using Services.Fixtures;

    public class DisplayNameAttributeFacts
    {
        [TestFixture]
        public class TheDisplayNameProperty
        {
            [TestCase]
            public void ReturnsTranslatedResourceName()
            {
                var languageService = new LanguageServiceFixture();
                languageService.RegisterValue("MyDisplayName", "It works");

                var displayAttribute = new Catel.ComponentModel.DisplayNameAttribute(languageService, "MyDisplayName");
 
                Assert.That(displayAttribute.DisplayName, Is.EqualTo("It works"));
            }

            [TestCase]
            public void ReturnsResourceNameIfTranslationCannotBeFound()
            {
                var languageService = new LanguageServiceFixture();
                languageService.RegisterValue("MyDisplayName", "It works");

                var displayAttribute = new Catel.ComponentModel.DisplayNameAttribute(languageService, "MyNonExistingDisplayName");

                Assert.That(displayAttribute.DisplayName, Is.EqualTo("MyNonExistingDisplayName"));
            }
        }
    }
}
