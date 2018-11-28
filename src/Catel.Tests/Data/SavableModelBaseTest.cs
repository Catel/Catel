// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavableModelBaseTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
#if NET || NETCORE
        [TestCase]
        public void Load_FileName_SerializationMode_Binary()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            var loadedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetBinarySerializer());

            Assert.AreEqual(originalObject, loadedObject);
        }
#endif

        [TestCase]
        public void Load_FileName_SerializationMode_Xml()
        {
            var originalObject = ModelBaseTestHelper.CreateIniFileObject();
            var loadedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

            Assert.AreEqual(originalObject, loadedObject);
        }

#if NET || NETCORE
        [TestCase]
        public void Load_Stream_SerializationMode_Binary()
        {
            // Note: in a perfect world, we would have real models deriving from SavableModelBase

            using (var memoryStream = new MemoryStream())
            {
                var serializer = SerializationFactory.GetBinarySerializer();

                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                serializer.Serialize(originalObject, memoryStream);

                memoryStream.Position = 0L;

                var loadedObject = serializer.Deserialize(typeof(IniFile), memoryStream);

                Assert.AreEqual(originalObject, loadedObject);
            }
        }
#endif

        [TestCase]
        public void Load_Stream_SerializationMode_Xml()
        {
            // Note: in a perfect world, we would have real models deriving from SavableModelBase

            using (var memoryStream = new MemoryStream())
            {
                var serializer = SerializationFactory.GetBinarySerializer();

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
