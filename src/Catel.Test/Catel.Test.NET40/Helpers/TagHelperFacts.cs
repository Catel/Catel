// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TagHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class TagHelperFacts
    {
        [TestClass]
        public class TheAreTagsEqualMethod
        {
            [TestMethod]
            public void ReturnsTrueForBothNull()
            {
                Assert.IsTrue(TagHelper.AreTagsEqual(null, null));
            }

            [TestMethod]
            public void ReturnsTrueForEqualStrings()
            {
                Assert.IsTrue(TagHelper.AreTagsEqual("Catel", "Catel"));
            }

            [TestMethod]
            public void ReturnsFalseForDifferentStrings()
            {
                Assert.IsFalse(TagHelper.AreTagsEqual("Catel", "mvvm"));
            }

            [TestMethod]
            public void ReturnsFalseForDifferentCasingStrings()
            {
                Assert.IsFalse(TagHelper.AreTagsEqual("Catel", "catel"));
            }

            [TestMethod]
            public void ReturnsTrueForEqualInstances()
            {
                IniEntry firstEntry = ModelBaseTestHelper.CreateIniEntryObject("A", "B", "C");
                IniEntry secondEntry = ModelBaseTestHelper.CreateIniEntryObject("A", "B", "C");

                // References equal
                Assert.IsTrue(TagHelper.AreTagsEqual(firstEntry, firstEntry));

                // Objects equal
                Assert.IsTrue(TagHelper.AreTagsEqual(firstEntry, secondEntry));
            }

            [TestMethod]
            public void ReturnsFalseForDifferentInstances()
            {
                IniEntry firstEntry = ModelBaseTestHelper.CreateIniEntryObject("A", "B", "C");
                IniEntry secondEntry = ModelBaseTestHelper.CreateIniEntryObject("D", "E", "F");

                Assert.IsFalse(TagHelper.AreTagsEqual(firstEntry, secondEntry));
            }
        }
    }
}