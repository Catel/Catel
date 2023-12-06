namespace Catel.Tests
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
                Assert.That(TagHelper.AreTagsEqual(null, null), Is.True);
            }

            [TestCase]
            public void ReturnsTrueForEqualStrings()
            {
                Assert.That(TagHelper.AreTagsEqual("Catel", "Catel"), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForDifferentStrings()
            {
                Assert.That(TagHelper.AreTagsEqual("Catel", "mvvm"), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForDifferentCasingStrings()
            {
                Assert.That(TagHelper.AreTagsEqual("Catel", "catel"), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForEqualInstances()
            {
                var firstEntry = ModelBaseTestHelper.CreateIniEntryObject("A", "B", "C");
                var secondEntry = ModelBaseTestHelper.CreateIniEntryObject("A", "B", "C");

                // References equal
                Assert.That(TagHelper.AreTagsEqual(firstEntry, firstEntry), Is.True);

                // Objects equal
                Assert.That(TagHelper.AreTagsEqual(firstEntry, secondEntry), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForDifferentInstances()
            {
                var firstEntry = ModelBaseTestHelper.CreateIniEntryObject("A", "B", "C");
                var secondEntry = ModelBaseTestHelper.CreateIniEntryObject("D", "E", "F");

                Assert.That(TagHelper.AreTagsEqual(firstEntry, secondEntry), Is.False);
            }
        }
    }
}