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
            public void Returns_True_For_Both_Null()
            {
                Assert.That(TagHelper.AreTagsEqual(null, null), Is.True);
            }

            [TestCase]
            public void Returns_True_For_Equal_Strings()
            {
                Assert.That(TagHelper.AreTagsEqual("Catel", "Catel"), Is.True);
            }

            [TestCase]
            public void Returns_False_For_Different_Strings()
            {
                Assert.That(TagHelper.AreTagsEqual("Catel", "mvvm"), Is.False);
            }

            [TestCase]
            public void Returns_False_For_Different_Casing_Strings()
            {
                Assert.That(TagHelper.AreTagsEqual("Catel", "catel"), Is.False);
            }

            [TestCase]
            public void Returns_True_For_Equal_Instances()
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
            public void Returns_False_For_Different_Instances()
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
