namespace Catel.Tests.Data
{
    using System.IO;
    using Catel.Runtime.Serialization;
    using Catel.Tests.Runtime.Serialization;

    using NUnit.Framework;

    [TestFixture]
    public class SavableModelBaseTest
    {
        #region Generic Loads
        [TestCase]
        public void Load_FileName_SerializationMode_Xml()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            var loadedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

            Assert.AreEqual(originalObject, loadedObject);
        }

        [TestCase]
        public void Load_Stream_SerializationMode_Xml()
        {
            // Note: in a perfect world, we would have real models deriving from SavableModelBase

            using (var memoryStream = new MemoryStream())
            {
                var serializer = SerializationFactory.GetXmlSerializer();

                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                serializer.Serialize(originalObject, memoryStream);

                memoryStream.Position = 0L;
                var loadedObject = serializer.Deserialize(typeof(IniFile), memoryStream);

                Assert.AreEqual(originalObject, loadedObject);
            }
        }
        #endregion
    }
}
