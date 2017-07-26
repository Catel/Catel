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
        public class TheSplitByCamelCaseMethod
        {
            [TestCase(null, null)]
            [TestCase("ThisIsATest", "This is a test")]
            [TestCase("IsIncluded", "Is included")]
            [TestCase("PropertyName", "Property name")]
            public void SplitsCorrectly(string input, string expectedOutput)
            {
                var output = input.SplitCamelCase();

                Assert.AreEqual(expectedOutput, output);
            }
        }

        [TestFixture]
        public class TheGetSlugMethod
        {
            [TestCase("this.hEllO something", "thishEllOsomething", false)]
            [TestCase("this.hEllO something", "thishellosomething", true)]
            [TestCase("tesTß", "tesT", false)]
            [TestCase("tesTß", "test", true)]
            [TestCase("tesT\\*&$", "tesT", false)]
            [TestCase("tesT\\*&$", "test", true)]
            public void ReturnsRightSlug(string input, string expectedOutput, bool lowercase)
            {
                var output = input.GetSlug(makeLowercase: lowercase);

                Assert.AreEqual(expectedOutput, output);
            }
        }
    }
}