namespace Catel.Tests.Data
{
    using NUnit.Framework;

    public partial class ComparableModelBaseFacts
    {
        [TestCase]
        public void Equals_Generic()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateIniEntryObject();
            var obj2 = ModelBaseTestHelper.CreateIniEntryObject();

            // Equals
            Assert.IsTrue(obj1.Equals(obj2));
            Assert.IsTrue(obj2.Equals(obj1));
        }

        [TestCase]
        public void Equals_Generic_Null()
        {
            // Create 2 objects
            IniEntry obj1 = ModelBaseTestHelper.CreateIniEntryObject();
            IniEntry obj2 = null;

            // Equals
            Assert.IsFalse(obj1.Equals(obj2));
        }

        [TestCase]
        public void Equals_DifferentClassesEqualProperties()
        {
            ClassWithoutPropertiesA a = new ClassWithoutPropertiesA();
            ClassWithoutPropertiesB b = new ClassWithoutPropertiesB();

            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests the Equals method 1 level deep.
        /// </summary>
        [TestCase]
        public void EqualsLevel1()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateIniEntryObject();
            var obj2 = ModelBaseTestHelper.CreateIniEntryObject();

            // Equals
            Assert.AreEqual(obj1, obj2);
        }

        /// <summary>
        /// Tests the Equals method 2 level deep.
        /// </summary>
        [TestCase]
        public void EqualsLevel2()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateIniFileObject();
            var obj2 = ModelBaseTestHelper.CreateIniFileObject();

            // Equals
            Assert.AreEqual(obj1, obj2);
        }

        /// <summary>
        /// Tests the Equals method 3 level deep.
        /// </summary>
        [TestCase]
        public void EqualsLevel3()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateComputerSettingsObject();
            var obj2 = ModelBaseTestHelper.CreateComputerSettingsObject();

            // Equals
            Assert.AreEqual(obj1, obj2);
        }

        [TestCase]
        public void Equals_AreNotEqual()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateComputerSettingsObject();
            var obj2 = ModelBaseTestHelper.CreateIniFileObject();

            // Equals
            Assert.AreNotEqual(obj1, obj2);
        }

    }
}