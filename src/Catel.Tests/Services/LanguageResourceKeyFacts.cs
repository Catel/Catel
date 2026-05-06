namespace Catel.Tests.Services;

using System.Globalization;
using Catel.Services;
using NUnit.Framework;

public class LanguageResourceKeyFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Sets_Properties_Correctly()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");

            var key = new LanguageResourceKey("SomeResource", culture);

            Assert.That(key.ResourceName, Is.EqualTo("SomeResource"));
            Assert.That(key.CultureInfo, Is.EqualTo(culture));
        }
    }

    [TestFixture]
    public class The_Equals_Method
    {
        [Test]
        public void Returns_True_For_Keys_With_Same_Values()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");
            var key1 = new LanguageResourceKey("SomeResource", culture);
            var key2 = new LanguageResourceKey("SomeResource", culture);

            Assert.That(key1.Equals(key2), Is.True);
        }

        [Test]
        public void Returns_False_For_Keys_With_Different_ResourceName()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");
            var key1 = new LanguageResourceKey("Resource1", culture);
            var key2 = new LanguageResourceKey("Resource2", culture);

            Assert.That(key1.Equals(key2), Is.False);
        }

        [Test]
        public void Returns_False_For_Null()
        {
            var key = new LanguageResourceKey("SomeResource", CultureInfo.InvariantCulture);

            Assert.That(key.Equals(null), Is.False);
        }
    }

    [TestFixture]
    public class The_GetHashCode_Method
    {
        [Test]
        public void Returns_Same_HashCode_For_Equal_Keys()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");
            var key1 = new LanguageResourceKey("SomeResource", culture);
            var key2 = new LanguageResourceKey("SomeResource", culture);

            Assert.That(key1.GetHashCode(), Is.EqualTo(key2.GetHashCode()));
        }
    }

    [TestFixture]
    public class The_ToString_Method
    {
        [Test]
        public void Returns_ResourceName_And_Culture()
        {
            var culture = CultureInfo.GetCultureInfo("en-US");
            var key = new LanguageResourceKey("SomeResource", culture);

            var result = key.ToString();

            Assert.That(result, Does.Contain("SomeResource"));
            Assert.That(result, Does.Contain("en-US"));
        }
    }
}
