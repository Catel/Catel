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
                Assert.AreEqual(2, obj4ReferenceInfo.Id);

                var obj3ReferenceInfo = referenceManager.GetInfo(obj3);
                Assert.AreEqual(4, obj3ReferenceInfo.Id);
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

                Assert.IsNull(referenceManager.GetInfoById(5));
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

                Assert.IsNotNull(referenceManager.GetInfoById(3));
            }
        }

        [TestFixture]
        public class TheGetInfoMethod
        {
            [TestCase]
            public void ReturnsNullForNullInstance()
            {
                var referenceManager = new ReferenceManager();

                Assert.IsNull(referenceManager.GetInfo(null, false));
            }

            [TestCase]
            public void ReturnsTrueForFirstUsageOnFirstUsage()
            {
                var referenceManager = new ReferenceManager();
                var referenceInfo = referenceManager.GetInfo(new object());

                Assert.IsTrue(referenceInfo.IsFirstUsage);
            }

            [TestCase]
            public void ReturnsFalseForFirstUsageOnSecondUsage()
            {
                var referenceManager = new ReferenceManager();
                var obj = new object();

                referenceManager.GetInfo(obj);
                var referenceInfo = referenceManager.GetInfo(obj);

                Assert.IsFalse(referenceInfo.IsFirstUsage);
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

                Assert.AreEqual(2, referenceInfo.Id);
                Assert.IsTrue(ReferenceEquals(obj2, referenceInfo.Instance));
            }
        }
    }
}
