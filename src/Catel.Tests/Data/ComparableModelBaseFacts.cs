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
            Assert.That(obj1, Is.EqualTo(obj2));
            Assert.That(obj2, Is.EqualTo(obj1));
        }

        [TestCase]
        public void Equals_Generic_Null()
        {
            // Create 2 objects
            IniEntry obj1 = ModelBaseTestHelper.CreateIniEntryObject();
            IniEntry obj2 = null;

            // Equals
            Assert.That(obj1, Is.Not.EqualTo(obj2));
        }

        [TestCase]
        public void Equals_DifferentClassesEqualProperties()
        {
            var a = new ClassWithoutPropertiesA();
            var b = new ClassWithoutPropertiesB();

#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
            Assert.That(b.Equals(a), Is.False);
            Assert.That(a.Equals(b), Is.False);
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
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
            Assert.That(obj2, Is.EqualTo(obj1));
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
            Assert.That(obj2, Is.EqualTo(obj1));
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
            Assert.That(obj2, Is.EqualTo(obj1));
        }

        [TestCase]
        public void Equals_AreNotEqual()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateComputerSettingsObject();
            var obj2 = ModelBaseTestHelper.CreateIniFileObject();

            // Equals
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
            Assert.That(obj2.Equals(obj1), Is.False);
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
        }
    }
}
