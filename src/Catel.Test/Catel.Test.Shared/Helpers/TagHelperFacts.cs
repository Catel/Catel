// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TagHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using Data;

    using NUnit.Framework;

    public class TagHelperFacts
    {
        [TestFixture]
        public class TheAreTagsEqualMethod
        {
            [TestCase]
            public void ReturnsTrueForBothNull()
            {
                Assert.IsTrue(TagHelper.AreTagsEqual(null, null));
            }

            [TestCase]
            public void ReturnsTrueForEqualStrings()
            {
                Assert.IsTrue(TagHelper.AreTagsEqual("Catel", "Catel"));
            }

            [TestCase]
            public void ReturnsFalseForDifferentStrings()
            {
                Assert.IsFalse(TagHelper.AreTagsEqual("Catel", "mvvm"));
            }

            [TestCase]
            public void ReturnsFalseForDifferentCasingStrings()
            {
                Assert.IsFalse(TagHelper.AreTagsEqual("Catel", "catel"));
            }

            [TestCase]
            public void ReturnsTrueForEqualInstances()
            {
                IniEntry firstEntry = ModelBaseTestHelper.CreateIniEntryObject("A", "B", "C");
                IniEntry secondEntry = ModelBaseTestHelper.CreateIniEntryObject("A", "B", "C");

                // References equal
                Assert.IsTrue(TagHelper.AreTagsEqual(firstEntry, firstEntry));

                // Objects equal
                Assert.IsTrue(TagHelper.AreTagsEqual(firstEntry, secondEntry));
            }

            [TestCase]
            public void ReturnsFalseForDifferentInstances()
            {
                IniEntry firstEntry = ModelBaseTestHelper.CreateIniEntryObject("A", "B", "C");
                IniEntry secondEntry = ModelBaseTestHelper.CreateIniEntryObject("D", "E", "F");

                Assert.IsFalse(TagHelper.AreTagsEqual(firstEntry, secondEntry));
            }
        }
    }
}