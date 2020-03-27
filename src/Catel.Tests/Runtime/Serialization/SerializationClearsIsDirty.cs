namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
            Assert.IsInstanceOf(typeof(IsDirtyModelTestModel), cm, "The IsDirtyModelTestModel constructor did not return the correct type.");
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

            // Round-trip the instance: Serialize and de-serialize with a BinaryFormatter
            var actual = SerializationTestHelper.SerializeAndDeserialize(input, SerializationFactory.GetXmlSerializer());

            // Double-check that the internal values are preserved
            Assert.AreEqual(input.MyDecimal, actual.MyDecimal, "MyDecimal values do not match.");
            Assert.AreEqual(input.MyInteger, actual.MyInteger, "MyInteger values do not match.");
            Assert.AreEqual(input.MyString, actual.MyString, "MyString values do not match.");
            //Assert.AreEqual(input.IsDirty, actual.IsDirty, "IsDirty values do not match.");
            Assert.IsFalse(actual.IsDirty);
        }
    }
}
