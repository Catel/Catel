// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayNameAttributeFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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

                var displayAttribute = new Catel.ComponentModel.DisplayNameAttribute("MyDisplayName");
                displayAttribute.LanguageService = languageService;

                Assert.AreEqual("It works", displayAttribute.DisplayName);
            }

            [TestCase]
            public void ReturnsResourceNameIfTranslationCannotBeFound()
            {
                var languageService = new LanguageServiceFixture();
                languageService.RegisterValue("MyDisplayName", "It works");

                var displayAttribute = new Catel.ComponentModel.DisplayNameAttribute("MyNonExistingDisplayName");
                displayAttribute.LanguageService = languageService;

                Assert.AreEqual("MyNonExistingDisplayName", displayAttribute.DisplayName);
            }
        }
    }
}