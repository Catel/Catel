namespace Catel.Tests.IoC
{
    using System;
    using Catel.IoC;
    using NUnit.Framework;

    public class TypeRequestInfoFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                Assert.Throws<ArgumentNullException>(() => new TypeRequestInfo(null));
            }

            [TestCase]
            public void SetsValuesCorrectly()
            {
                var typeRequestInfo = new TypeRequestInfo(typeof(int), "mytag");

                Assert.That(typeRequestInfo.Type, Is.EqualTo(typeof(int)));
                Assert.That(typeRequestInfo.Tag, Is.EqualTo("mytag"));
            }
        }

        [TestFixture]
        public class TheComparisonMethods
        {
            [TestCase]
            public void FunctionsCorrectlyForEqualTypes()
            {
                var obj1 = new TypeRequestInfo(typeof(int), "mytag");
                var obj2 = new TypeRequestInfo(typeof(int), "mytag");

                Assert.That(obj1, Is.EqualTo(obj2));
                Assert.That(obj2, Is.EqualTo(obj1));

                Assert.That(obj1, Is.EqualTo(obj2));
                Assert.That(obj2, Is.EqualTo(obj1));

                Assert.That(obj1, Is.EqualTo(obj2));
                Assert.That(obj2, Is.EqualTo(obj1));
            }

            [TestCase]
            public void FunctionsCorrectlyForDifferentTypes()
            {
                var obj1 = new TypeRequestInfo(typeof(int), "mytag");
                var obj2 = new TypeRequestInfo(typeof(double), "mytag");

                Assert.That(obj1, Is.Not.EqualTo(obj2));
                Assert.That(obj2, Is.Not.EqualTo(obj1));

                Assert.That(obj1, Is.Not.EqualTo(obj2));
                Assert.That(obj2, Is.Not.EqualTo(obj1));

                Assert.That(obj1, Is.Not.EqualTo(obj2));
                Assert.That(obj2, Is.Not.EqualTo(obj1));
            }

            [TestCase]
            public void FunctionsCorrectlyForDifferentTags()
            {
                var obj1 = new TypeRequestInfo(typeof(int), "mytag1");
                var obj2 = new TypeRequestInfo(typeof(int), "mytag2");

                Assert.That(obj1, Is.Not.EqualTo(obj2));
                Assert.That(obj2, Is.Not.EqualTo(obj1));

                Assert.That(obj1, Is.Not.EqualTo(obj2));
                Assert.That(obj2, Is.Not.EqualTo(obj1));

                Assert.That(obj1, Is.Not.EqualTo(obj2));
                Assert.That(obj2, Is.Not.EqualTo(obj1));
            }
        }
    }
}
