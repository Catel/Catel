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
                var firstEntry = new PersonTestModel
                {
                    FirstName = "test"
                };
                var secondEntry = new PersonTestModel
                {
                    FirstName = "test"
                };

                // References equal
                Assert.That(TagHelper.AreTagsEqual(firstEntry, firstEntry), Is.True);

                // Objects equal
                Assert.That(TagHelper.AreTagsEqual(firstEntry, secondEntry), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForDifferentInstances()
            {
                var firstEntry = new PersonTestModel
                {
                    FirstName = "test"
                };
                var secondEntry = new PersonTestModel
                {
                    FirstName = "test b"
                };

                Assert.That(TagHelper.AreTagsEqual(firstEntry, secondEntry), Is.False);
            }
        }
    }
}
