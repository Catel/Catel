namespace Catel.Tests.Runtime
{
    using Catel.Runtime;

    using NUnit.Framework;

    public class ReferenceManagerFacts
    {
        [TestFixture]
        public class TheRegisterManuallyMethod
        {
            [TestCase]
            public void RegistersManualIds()
            {
                var obj1 = new object();
                var obj2 = new object();
                var obj3 = new object();
                var obj4 = new object();

                var referenceManager = new ReferenceManager();

                referenceManager.RegisterManually(2, obj4);

                referenceManager.GetInfo(obj1);
                referenceManager.GetInfo(obj2);
                referenceManager.GetInfo(obj3);

                var obj4ReferenceInfo = referenceManager.GetInfo(obj4);
                Assert.That(obj4ReferenceInfo.Id, Is.EqualTo(2));

                var obj3ReferenceInfo = referenceManager.GetInfo(obj3);
                Assert.That(obj3ReferenceInfo.Id, Is.EqualTo(4));
            }
        }

        [TestFixture]
        public class TheGetInfoByIdMethod
        {
            [TestCase]
            public void ReturnsNullForNonExistingId()
            {
                var obj1 = new object();
                var obj2 = new object();
                var obj3 = new object();

                var referenceManager = new ReferenceManager();

                referenceManager.GetInfo(obj1);
                referenceManager.GetInfo(obj2);
                referenceManager.GetInfo(obj3);

                Assert.That(referenceManager.GetInfoById(5), Is.Null);
            }

            [TestCase]
            public void ReturnsReferenceInfoForNonExistingId()
            {
                var obj1 = new object();
                var obj2 = new object();
                var obj3 = new object();

                var referenceManager = new ReferenceManager();

                referenceManager.GetInfo(obj1);
                referenceManager.GetInfo(obj2);
                referenceManager.GetInfo(obj3);

                Assert.That(referenceManager.GetInfoById(3), Is.Not.Null);
            }
        }

        [TestFixture]
        public class TheGetInfoMethod
        {
            [TestCase]
            public void ReturnsNullForNullInstance()
            {
                var referenceManager = new ReferenceManager();

                Assert.That(referenceManager.GetInfo(null, false), Is.Null);
            }

            [TestCase]
            public void ReturnsTrueForFirstUsageOnFirstUsage()
            {
                var referenceManager = new ReferenceManager();
                var referenceInfo = referenceManager.GetInfo(new object());

                Assert.That(referenceInfo.IsFirstUsage, Is.True);
            }

            [TestCase]
            public void ReturnsFalseForFirstUsageOnSecondUsage()
            {
                var referenceManager = new ReferenceManager();
                var obj = new object();

                referenceManager.GetInfo(obj);
                var referenceInfo = referenceManager.GetInfo(obj);

                Assert.That(referenceInfo.IsFirstUsage, Is.False);
            }

            [TestCase]
            public void ReturnsSameInfoForSameReference()
            {
                var obj1 = new object();
                var obj2 = new object();
                var obj3 = new object();

                var referenceManager = new ReferenceManager();
                referenceManager.GetInfo(obj1);
                referenceManager.GetInfo(obj2);
                referenceManager.GetInfo(obj3);

                var referenceInfo = referenceManager.GetInfo(obj2);

                Assert.That(referenceInfo.Id, Is.EqualTo(2));
                Assert.That(ReferenceEquals(obj2, referenceInfo.Instance), Is.True);
            }
        }
    }
}
