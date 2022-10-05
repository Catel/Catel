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

                Assert.AreEqual(typeof(int), typeRequestInfo.Type);
                Assert.AreEqual("mytag", typeRequestInfo.Tag);
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

                Assert.IsTrue(obj1 == obj2);
                Assert.IsTrue(obj2 == obj1);

                Assert.IsFalse(obj1 != obj2);
                Assert.IsFalse(obj2 != obj1);

                Assert.IsTrue(obj1.Equals(obj2));
                Assert.IsTrue(obj2.Equals(obj1));
            }

            [TestCase]
            public void FunctionsCorrectlyForDifferentTypes()
            {
                var obj1 = new TypeRequestInfo(typeof(int), "mytag");
                var obj2 = new TypeRequestInfo(typeof(double), "mytag");

                Assert.IsFalse(obj1 == obj2);
                Assert.IsFalse(obj2 == obj1);

                Assert.IsTrue(obj1 != obj2);
                Assert.IsTrue(obj2 != obj1);

                Assert.IsFalse(obj1.Equals(obj2));
                Assert.IsFalse(obj2.Equals(obj1));
            }

            [TestCase]
            public void FunctionsCorrectlyForDifferentTags()
            {
                var obj1 = new TypeRequestInfo(typeof(int), "mytag1");
                var obj2 = new TypeRequestInfo(typeof(int), "mytag2");

                Assert.IsFalse(obj1 == obj2);
                Assert.IsFalse(obj2 == obj1);

                Assert.IsTrue(obj1 != obj2);
                Assert.IsTrue(obj2 != obj1);

                Assert.IsFalse(obj1.Equals(obj2));
                Assert.IsFalse(obj2.Equals(obj1));
            }
        }
    }
}
