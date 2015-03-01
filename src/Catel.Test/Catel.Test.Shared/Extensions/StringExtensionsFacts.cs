// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Extensions
{
    using NUnit.Framework;

    public class StringExtensionsFacts
    {
        [TestFixture]
        public class TheGetSlugMethod
        {
            [TestCase("this.hello something", "thishellosomething")]
            [TestCase("testß", "test")]
            [TestCase("test\\*&$", "test")]
            public void ReturnsRightSlug(string input, string expectedOutput)
            {
                var output = input.GetSlug();

                Assert.AreEqual(expectedOutput, output);
            }
        }
    }
}