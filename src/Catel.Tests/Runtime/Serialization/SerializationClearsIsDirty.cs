namespace Catel.Tests.Runtime.Serialization
{
    using Catel.Runtime.Serialization;
    using Catel.Tests.Runtime.Serialization.TestModels;
    using NUnit.Framework;

    [TestFixture]
    public class SerializationClearsIsDirty
    {
        [Test]
        public void MyCatelModel_ConstructorTest()
        {
            var cm = new IsDirtyModelTestModel();
            Assert.IsNotNull(cm, "The IsDirtyModelTestModel constructor returned a null instance.");
            Assert.That(cm, Is.InstanceOf(typeof(IsDirtyModelTestModel)), "The IsDirtyModelTestModel constructor did not return the correct type.");
        }

        [Test]
        public void MyCatelModel_SerializationTest()
        {
            var input = new IsDirtyModelTestModel
            {
                MyDecimal = 123.4567m,
                MyInteger = 98765,
                MyString = "This is a serialization test."
            };

            // Round-trip the instance: Serialize and de-serialize
            var actual = SerializationTestHelper.SerializeAndDeserialize(input, SerializationFactory.GetXmlSerializer());

            // Double-check that the internal values are preserved
            Assert.That(actual.MyDecimal, Is.EqualTo(input.MyDecimal), "MyDecimal values do not match.");
            Assert.That(actual.MyInteger, Is.EqualTo(input.MyInteger), "MyInteger values do not match.");
            Assert.That(actual.MyString, Is.EqualTo(input.MyString), "MyString values do not match.");
            //Assert.AreEqual(input.IsDirty, actual.IsDirty, "IsDirty values do not match.");
            Assert.That(actual.IsDirty, Is.False);
        }
    }
}
