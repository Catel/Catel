// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayNameAttributeFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.ComponentModel
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

                var displayAttribute = new DisplayNameAttribute("MyDisplayName");

                Assert.AreEqual("It works", displayAttribute.DisplayName);
            }
        }
    }
}